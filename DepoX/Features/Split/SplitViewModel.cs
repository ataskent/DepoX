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
    // UI STATE
    // ===============================

    public string? ErrorMessage { get; private set; }
    public string? InfoMessage { get; private set; }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool HasInfo => !string.IsNullOrWhiteSpace(InfoMessage);

    public async Task CreateNewBarcodeAsync()
    {
        ClearMessages();

        if (DraftRow == null)
        {
            SetError("Kaydedilecek bir barkod yok.");
            return;
        }

        if (DraftRow.Quantity <= 0)
        {
            SetError("Miktar 0 veya negatif olamaz.");
            return;
        }

        if (string.IsNullOrWhiteSpace(DraftRow.ItemCode))
        {
            SetError("Stok kodu boþ olamaz.");
            return;
        }

        if (string.IsNullOrWhiteSpace(DraftRow.UnitCode))
        {
            SetError("Birim kodu boþ olamaz.");
            return;
        }

        var draft = SplitMapper.ToNewBarcodeDraft(DraftRow);

        var result = await _service.CreateNewBarcodeAsync(draft);

        if (!result.Success)
        {
            SetError(result.Message ?? "Kaydetme iþlemi baþarýsýz.");
            return;
        }

        // Close draft
        DraftRow = null;
        OnPropertyChanged(nameof(DraftRow));
        OnPropertyChanged(nameof(IsDraftOpen));

        // Show success message
        SetInfo(result.Message ?? "Ýþlem baþarýyla tamamlandý.");
    }

    void SetError(string message)
    {
        ErrorMessage = message;
        InfoMessage = null;
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(HasInfo));
    }

    void SetInfo(string message)
    {
        InfoMessage = message;
        ErrorMessage = null;
        OnPropertyChanged(nameof(InfoMessage));
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(HasInfo));
    }

    void ClearMessages()
    {
        ErrorMessage = null;
        InfoMessage = null;
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(InfoMessage));
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(HasInfo));
    }

    public DraftMode CurrentDraftMode { get; set; }
    public string DraftTitle =>
    CurrentDraftMode == DraftMode.Split
        ? "Barkod Böl"
        : "Yeni Barkod Oluþtur";


    // ===============================
    // DATA
    // ===============================

    private SplitBarcodeModel? _loadedModel;

    public SplitRowVm? Original { get; private set; }

    bool _hasOriginalBarcode;
    public bool HasOriginalBarcode
    {
        get => _hasOriginalBarcode;
        private set
        {
            if (_hasOriginalBarcode == value) return;
            _hasOriginalBarcode = value;
            OnPropertyChanged();
        }
    }

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
    public bool IsDraftOpen => DraftRow != null;

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
    // LOAD
    // ===============================

    public async Task LoadAsync(string barcode)
    {
        ClearAll();
        ClearMessages();

        OriginalBarcode = barcode;

        var result = await _service.GetBarcodeAsync(barcode);

        if (!result.Success)
        {
            SetError(result.Message ?? "Barkod bilgisi alýnamadý.");
            return;
        }

        var model = result.Data!;
        _loadedModel = model;

        Original = SplitMapper.ToVm(model, true);
        HasOriginalBarcode = true;

        OnPropertyChanged(nameof(Original));
        OnPropertyChanged(nameof(OriginalLine2));

        foreach (var e in model.ExistingSplits)
            ExistingSplits.Add(SplitMapper.ToVm(e, true));
    }

    // ===============================
    // NEW SPLIT
    // ===============================

    public void StartNewBarcode()
    {
        CurrentDraftMode = DraftMode.NewBarcode;

        DraftRow = new SplitRowVm
        {
            Quantity = 1
        };

        OnPropertyChanged(nameof(CurrentDraftMode));
        OnPropertyChanged(nameof(DraftRow));
        OnPropertyChanged(nameof(IsDraftOpen));
        OnPropertyChanged(nameof(DraftTitle));
    }

    public void StartNewSplit()
    {
        if (Original == null || _loadedModel == null)
            return;

        CurrentDraftMode = DraftMode.Split;

        DraftRow = new SplitRowVm
        {
            IsExisting = false,
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

        OnPropertyChanged(nameof(CurrentDraftMode));
        OnPropertyChanged(nameof(DraftRow));
        OnPropertyChanged(nameof(IsDraftOpen));
        OnPropertyChanged(nameof(DraftTitle));
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

        bool sameAsOriginal =
            DraftRow.LotCode == Original.LotCode &&
            DraftRow.ColorCode == Original.ColorCode &&
            DraftRow.UnitCode == Original.UnitCode &&
            DraftRow.Quantity == Original.Quantity;

        if (sameAsOriginal)
        {
            SetError("Orijinal barkod ile birebir ayný kayýt eklenemez.");
            return;
        }

        NewSplits.Add(new SplitRowVm
        {
            IsExisting = false,
            ItemCode = DraftRow.ItemCode,
            ItemName = DraftRow.ItemName,
            LotCode = DraftRow.LotCode,
            ColorCode = ExtractCode(DraftRow.ColorCode),
            UnitCode = DraftRow.UnitCode,
            Quantity = DraftRow.Quantity
        });

        DraftRow = null;
        OnPropertyChanged(nameof(DraftRow));
        OnPropertyChanged(nameof(IsDraftOpen));
    }

    public void CancelDraft()
    {
        DraftRow = null;
        OnPropertyChanged(nameof(DraftRow));
        OnPropertyChanged(nameof(IsDraftOpen));
    }

    // ===============================
    // DELETE
    // ===============================

    public void Remove(SplitRowVm row)
    {
        if (row.IsExisting)
            return;

        NewSplits.Remove(row);
    }

    // ===============================
    // SAVE
    // ===============================

    public async Task SaveAsync()
    {
        ClearMessages();

        if (Original == null)
        {
            SetError("Kaydedilecek bir barkod yok.");
            return;
        }

        if (!NewSplits.Any())
        {
            SetError("Yeni eklenen barkod yok.");
            return;
        }

        var draft = SplitMapper.ToErpDraft(Original.Barcode, NewSplits);

        var result = await _service.SaveAsync(draft);

        if (!result.Success)
        {
            SetError(result.Message ?? "Kaydetme iþlemi baþarýsýz.");
            return;
        }

        // ?? ÖNCE reload
        await LoadAsync(Original.Barcode);

        // ? EN SON baþarý mesajý
        SetInfo(result.Message ?? "Ýþlem baþarýyla tamamlandý.");
    }

    // ===============================
    // HELPERS
    // ===============================

    void ClearAll()
    {
        HasOriginalBarcode = false;
        Original = null;
        _loadedModel = null;

        ExistingSplits.Clear();
        NewSplits.Clear();

        OnPropertyChanged(nameof(Original));
        OnPropertyChanged(nameof(OriginalLine2));
        OnPropertyChanged(nameof(MixedItems));
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
