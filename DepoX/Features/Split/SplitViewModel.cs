using DepoX.Services.Erp.Dtos;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DepoX.Features.Split;

public class SplitViewModel : INotifyPropertyChanged
{
    private readonly ISplitService _service;

    string ExtractCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        var idx = value.IndexOf('-');
        return idx > 0
            ? value.Substring(0, idx).Trim()
            : value.Trim();
    }

    // ===============================
    // UI MESSAGE STATE
    // ===============================

    public bool IsNewBarcodeMode => CurrentDraftMode == DraftMode.NewBarcode;

    //public bool ShowBottomSaveButton =>
    //CurrentDraftMode == DraftMode.Split && HasOriginalBarcode;



    public string? ErrorMessage { get; private set; }
    public string? InfoMessage { get; private set; }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool HasInfo => !string.IsNullOrWhiteSpace(InfoMessage);

    void SetError(string message)
    {
        ErrorMessage = message;
        InfoMessage = null;
        Notify(nameof(ErrorMessage), nameof(InfoMessage), nameof(HasError), nameof(HasInfo));
    }

    void SetInfo(string message)
    {
        InfoMessage = message;
        ErrorMessage = null;
        Notify(nameof(ErrorMessage), nameof(InfoMessage), nameof(HasError), nameof(HasInfo));
    }

    void ClearMessages()
    {
        ErrorMessage = null;
        InfoMessage = null;
        Notify(nameof(ErrorMessage), nameof(InfoMessage), nameof(HasError), nameof(HasInfo));
    }

    // ===============================
    // DRAFT MODE
    // ===============================

    private DraftMode _currentDraftMode;

    public DraftMode CurrentDraftMode
    {
        get => _currentDraftMode;
        set
        {
            if (_currentDraftMode != value)
            {
                _currentDraftMode = value;
                Notify(
                    nameof(CurrentDraftMode),
                    nameof(IsNewBarcodeMode),
                    nameof(IsSplitMode),
                    nameof(ShowBottomSaveButton)
                );
            }
        }
    }

    public bool IsSplitMode => CurrentDraftMode == DraftMode.Split;


    public string DraftTitle =>
        CurrentDraftMode == DraftMode.Split
            ? "Barkod Böl"
            : "Yeni Barkod Oluştur";

    // ===============================
    // DATA
    // ===============================

    private SplitBarcodeModel? _loadedModel;


    public bool ShowBottomSaveButton =>
    HasOriginalBarcode
    && CurrentDraftMode == DraftMode.Split
    && !IsDraftOpen;

    public bool ShowNewBarcodeFields => CurrentDraftMode == DraftMode.NewBarcode;
    public bool ShowSplitFields => CurrentDraftMode == DraftMode.Split;



    public SplitRowVm? Original { get; private set; }
    public bool HasOriginalBarcode => Original != null;

    public string OriginalLine2 =>
        Original == null
            ? ""
            : $"Parti: {Original.LotCode} · Renk: {Original.ColorCode} · Miktar: {Original.Quantity} {Original.UnitCode}";

    public ObservableCollection<SplitRowVm> ExistingSplits { get; } = new();
    public ObservableCollection<SplitRowVm> NewSplits { get; } = new();

    public IEnumerable<SplitRowVm> MixedItems =>
        ExistingSplits.Concat(NewSplits);

    // Draft
    public SplitRowVm? DraftRow { get; private set; }

    private bool _isDraftOpen;
    public bool IsDraftOpen
    {
        get => _isDraftOpen;
        set
        {
            if (_isDraftOpen != value)
            {
                _isDraftOpen = value;
                Notify(
                    nameof(IsDraftOpen),
                    nameof(ShowBottomSaveButton)
                );
            }
        }
    }


    public string OriginalBarcode { get; private set; } = "";

    // ===============================
    // CTOR
    // ===============================

    public SplitViewModel(ISplitService service)
    {
        _service = service;

        ExistingSplits.CollectionChanged += (_, __) =>
            OnPropertyChanged(nameof(MixedItems));

        NewSplits.CollectionChanged += (_, __) =>
            OnPropertyChanged(nameof(MixedItems));
    }

    // ===============================
    // DELETE (Swipe ile silme)
    // ===============================
    public void Remove(SplitRowVm row)
    {
        if (row == null)
            return;

        // ERP'den gelen eski splitler silinemez
        if (row.IsExisting)
            return;

        if (NewSplits.Contains(row))
            NewSplits.Remove(row);
    }


    // ===============================
    // LOAD BARCODE
    // ===============================

    public async Task LoadAsync(string barcode)
    {
        ClearAll();
        ClearMessages();

        OriginalBarcode = barcode;

        var result = await _service.GetBarcodeAsync(barcode);
        if (!result.Success)
        {
            SetError(result.Message ?? "Barkod bilgisi alınamadı.");
            return;
        }

        _loadedModel = result.Data!;
        Original = SplitMapper.ToVm(_loadedModel, true);

        OnPropertyChanged(nameof(Original));
        OnPropertyChanged(nameof(OriginalLine2));
        OnPropertyChanged(nameof(HasOriginalBarcode));

        foreach (var e in _loadedModel.ExistingSplits)
            ExistingSplits.Add(SplitMapper.ToVm(e, true));
    }

    /// <summary>
    /// Initializes a new barcode entry by retrieving item, color, and unit metadata asynchronously and preparing the
    /// draft state for user input.
    /// </summary>
    /// <remarks>This method resets the current page and clears any existing messages before starting a new
    /// barcode entry. It updates the draft state with metadata retrieved from the service. If the metadata cannot be
    /// retrieved, an error message is set and the draft is not initialized.</remarks>
    /// <returns></returns>
    public async Task StartNewBarcodeAsync()
    {
        ClearMessages();
        ResetPage();

        CurrentDraftMode = DraftMode.NewBarcode;

        // 1️⃣ ERP'den stok/renk/birim çek
        var meta = await _service.GetNewBarcodeMetaAsync();
        if (!meta.Success)
        {
            SetError(meta.Message ?? "Stok bilgileri alınamadı.");
            return;
        }

        DraftRow = new SplitRowVm
        {
            IsNewBarcodeMode = true,
            Quantity = 1,
            ItemList = meta.Data!.Items,
            ColorList = meta.Data.Colors,
            UnitList = meta.Data.Units,
            LotList = new List<string>() // stok seçilince dolacak
        };

        IsDraftOpen = true;

        Notify(
            nameof(CurrentDraftMode),
            nameof(DraftRow),
            nameof(IsDraftOpen),
            nameof(DraftTitle),
            nameof(ShowBottomSaveButton),
            nameof(IsNewBarcodeMode)
        );
    }


    /// <summary>
    /// Resat Page
    /// </summary>
    void ResetPage()
    {
        Original = null;
        _loadedModel = null;
        OriginalBarcode = "";

        ExistingSplits.Clear();
        NewSplits.Clear();

        DraftRow = null;
        CurrentDraftMode = DraftMode.Split;

        Notify(
            nameof(Original),
            nameof(HasOriginalBarcode),
            nameof(OriginalLine2),
            nameof(MixedItems),
            nameof(DraftRow),
            nameof(IsDraftOpen),
            nameof(CurrentDraftMode),
            nameof(DraftTitle),
            nameof(ShowBottomSaveButton)
        );
    }

    // ===============================
    // NEW BARCODE
    // ===============================

    public void StartNewBarcode()
    {
        CurrentDraftMode = DraftMode.NewBarcode;

        DraftRow = new SplitRowVm
        {
            IsNewBarcodeMode = true,
            Quantity = 1,
            ItemList = new List<ItemMetaDto>()
        };

        Notify(nameof(CurrentDraftMode), nameof(DraftRow), nameof(IsDraftOpen), nameof(DraftTitle), nameof(IsNewBarcodeMode));
    }

    public async Task CreateNewBarcodeAsync()
    {
        ClearMessages();

        if (DraftRow == null)
        {
            SetError("Kaydedilecek barkod yok.");
            return;
        }

        if (string.IsNullOrWhiteSpace(DraftRow.NewBarcode))
        {
            SetError("Barkod numarası girilmelidir.");
            return;
        }

        if (string.IsNullOrWhiteSpace(DraftRow.ItemCode))
        {
            SetError("Stok seçilmelidir.");
            return;
        }

        if (DraftRow.Quantity <= 0)
        {
            SetError("Miktar 0 veya negatif olamaz.");
            return;
        }

        if (string.IsNullOrWhiteSpace(DraftRow.UnitCode))
        {
            SetError("Birim seçilmelidir.");
            return;
        }

        var draft = SplitMapper.ToNewBarcodeDraft(DraftRow);
        var result = await _service.CreateNewBarcodeAsync(draft);

        if (!result.Success)
        {
            SetError(result.Message ?? "Yeni barkod oluşturulamadı.");
            return;
        }

        DraftRow = null;
        Notify(nameof(DraftRow), nameof(IsDraftOpen));

        SetInfo(result.Message ?? "Yeni barkod başarıyla oluşturuldu.");
    }

    public async Task ConfirmNewBarcodeAsync()
    {
        ClearMessages();

        if (DraftRow == null)
        {
            SetError("Kaydedilecek barkod yok.");
            return;
        }

        var draft = SplitMapper.ToNewBarcodeDraft(DraftRow);
        var result = await _service.CreateNewBarcodeAsync(draft);

        if (!result.Success)
        {
            SetError(result.Message ?? "Yeni barkod oluşturulamadı.");
            return;
        }

        // 🔥 SERVİSTEN GELEN VERİYİ DİREKT KULLAN
        _loadedModel = result.Data!;

        Original = SplitMapper.ToVm(_loadedModel, true);
        OriginalBarcode = Original.Barcode;

        ExistingSplits.Clear();
        NewSplits.Clear();

        DraftRow = null;
        IsDraftOpen = false;
        CurrentDraftMode = DraftMode.Split;

        Notify(
            nameof(Original),
            nameof(OriginalBarcode),
            nameof(HasOriginalBarcode),
            nameof(OriginalLine2),
            nameof(MixedItems),
            nameof(CurrentDraftMode),
            nameof(ShowBottomSaveButton)
        );

        SetInfo(result.Message ?? "Yeni barkod başarıyla oluşturuldu.");
    }


    //public async Task ConfirmNewBarcodeAsync()
    //{
    //    ClearMessages();

    //    if (DraftRow == null)
    //    {
    //        SetError("Kaydedilecek barkod yok.");
    //        return;
    //    }

    //    var draft = SplitMapper.ToNewBarcodeDraft(DraftRow);
    //    var result = await _service.CreateNewBarcodeAsync(draft);

    //    if (!result.Success)
    //    {
    //        SetError(result.Message ?? "Yeni barkod oluşturulamadı.");
    //        return;
    //    }

    //    ResetPage(); // ✅ sadece burada
    //    SetInfo(result.Message ?? "Yeni barkod başarıyla oluşturuldu.");
    //}


    bool _isItemPickerOpen = false;
    public bool IsItemPickerOpen
    {
        get => _isItemPickerOpen;
        set { _isItemPickerOpen = value; OnPropertyChanged(); }
    }


    public async Task LoadLotsForSelectedItemAsync(string itemCode)
    {
        if (DraftRow == null)
            return;

        var result = await _service.GetLotsByItemAsync(itemCode);

        if (!result.Success)
        {
            SetError(result.Message ?? "Partiler alınamadı.");
            return;
        }

        DraftRow.LotList = result.Data!;
        DraftRow.OnPropertyChanged(nameof(DraftRow.LotList));
    }





    // ===============================
    // NEW SPLIT
    // ===============================

    public void StartNewSplit()
    {
        if (Original == null || _loadedModel == null)
            return;

        CurrentDraftMode = DraftMode.Split;

        DraftRow = new SplitRowVm
        {
            IsNewBarcodeMode = false,
            ItemCode = Original.ItemCode,
            ItemName = Original.ItemName,
            LotCode = Original.LotCode,
            ColorCode = Original.ColorCode,
            UnitCode = Original.UnitCode,
            Quantity = 0,

            LotList = new List<string> { "" }.Concat(_loadedModel.AvailableLots).ToList(),
            ColorList = new List<string> { "" }.Concat(_loadedModel.AvailableColors).ToList(),
            UnitList = new List<string> { "" }.Concat(_loadedModel.AvailableUnits).ToList()
        };

        DraftRow.OnItemChanged = async itemCode =>
        {
            if (string.IsNullOrWhiteSpace(itemCode))
                return;

            var lots = await _service.GetLotsByItemAsync(itemCode);
            if (!lots.Success)
            {
                SetError(lots.Message ?? "Partiler alınamadı.");
                return;
            }

            DraftRow.LotList = lots.Data!;
            DraftRow.OnPropertyChanged(nameof(DraftRow.LotList));
        };

        IsDraftOpen = true; // 🔴 EN KRİTİK SATIR

        Notify(
            nameof(CurrentDraftMode),
            nameof(DraftRow),
            nameof(IsDraftOpen),
            nameof(DraftTitle),
            nameof(IsNewBarcodeMode),
            nameof(ShowBottomSaveButton)
        );
    }

    public void ConfirmDraft()
    {
        if (DraftRow == null || Original == null)
            return;

        if (DraftRow.Quantity <= 0)
        {
            SetError("Miktar 0 veya negatif olamaz.");
            return;
        }

        NewSplits.Add(new SplitRowVm
        {
            ItemCode = DraftRow.ItemCode,
            ItemName = DraftRow.ItemName,
            LotCode = DraftRow.LotCode,
            ColorCode = ExtractCode(DraftRow.ColorCode),
            UnitCode = DraftRow.UnitCode,
            Quantity = DraftRow.Quantity
        });

        DraftRow = null;
        IsDraftOpen = false;
        Notify(nameof(DraftRow), nameof(IsDraftOpen));
    }

    public void CancelDraft()
    {
        DraftRow = null;
        IsDraftOpen = false;
        // 👉 otomatik olarak ShowBottomSaveButton = true olur

        Notify(nameof(DraftRow), nameof(IsDraftOpen));

        if (CurrentDraftMode == DraftMode.NewBarcode)
        {
            // yeni barkoddan vazgeçildi → her şeyi temizle
            ResetPage();
        }
        // Split moddaysa SADECE popup kapanır
    }


    // ===============================
    // SAVE SPLIT
    // ===============================

    public async Task SaveAsync()
    {
        ClearMessages();

        if (Original == null)
        {
            SetError("Kaydedilecek barkod yok.");
            return;
        }

        if (!NewSplits.Any())
        {
            SetError("Yeni barkod eklenmedi.");
            return;
        }

        var draft = SplitMapper.ToErpDraft(Original.Barcode, NewSplits);
        var result = await _service.SaveAsync(draft);

        if (!result.Success)
        {
            SetError(result.Message ?? "Barkod bölme kaydedilemedi.");
            return;
        }

        await LoadAsync(Original.Barcode);
        SetInfo(result.Message ?? "Barkod bölme başarıyla kaydedildi.");
    }

    // ===============================
    // HELPERS
    // ===============================

    void ClearAll()
    {
        Original = null;
        _loadedModel = null;
        ExistingSplits.Clear();
        NewSplits.Clear();

        Notify(nameof(Original), nameof(OriginalLine2), nameof(HasOriginalBarcode), nameof(MixedItems));
    }

    void Notify(params string[] props)
    {
        foreach (var p in props)
            OnPropertyChanged(p);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public enum DraftMode
{
    Split,
    NewBarcode
}
