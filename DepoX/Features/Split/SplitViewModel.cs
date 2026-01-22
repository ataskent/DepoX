using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DepoX.Features.Split;

public class SplitViewModel : INotifyPropertyChanged
{
    private readonly ISplitService _service;

    // 🔴 ORİJİNAL (TEK NESNE)

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


    // ⚪ ESKİ SPLITLER (PASİF)
    public ObservableCollection<SplitRowVm> ExistingSplits { get; } = new();

    // 🟢 YENİ SPLITLER (AKTİF)
    public ObservableCollection<SplitRowVm> NewSplits { get; } = new();

    // UI için birleşik liste
    public IEnumerable<SplitRowVm> MixedItems =>
        ExistingSplits.Concat(NewSplits);

    // Draft popup
    public SplitRowVm? DraftRow { get; private set; }
    public bool IsDraftOpen => DraftRow != null;

    public string OriginalBarcode { get; private set; } = "";

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
        // 🔴 EN BAŞTA: her şeyi kapat
        HasOriginalBarcode = false;
        Original = null;
        _loadedModel = null;

        ExistingSplits.Clear();
        NewSplits.Clear();

        OnPropertyChanged(nameof(Original));
        OnPropertyChanged(nameof(OriginalLine2));
        OnPropertyChanged(nameof(MixedItems));

        SplitBarcodeModel model;

        try
        {
            model = await _service.GetBarcodeAsync(barcode);
        }
        catch
        {
            // barkod bulunamadı → UI kapalı kalır
            return;
        }

        if (model == null)
            return;

        // ✅ SADECE BAŞARILIYSA
        _loadedModel = model;
        Original = SplitMapper.ToVm(model, true);
        HasOriginalBarcode = true;

        OnPropertyChanged(nameof(Original));
        OnPropertyChanged(nameof(OriginalLine2));

        foreach (var e in model.ExistingSplits)
            ExistingSplits.Add(SplitMapper.ToVm(e, true));
    }



    // ===============================
    // NEW SPLIT (POPUP)
    // ===============================
    public void StartNewSplit()
    {
        if (Original == null || _loadedModel == null)
            return;

        DraftRow = new SplitRowVm
        {
            IsExisting = false,

            ItemCode = Original.ItemCode,
            ItemName = Original.ItemName,

            // 🔴 Varsayılanlar
            LotCode = Original.LotCode,
            ColorCode = Original.ColorCode,
            UnitCode = Original.UnitCode,
            Quantity = 0,

            // 🔴 ERP’den gelen listeler
            LotList = new List<string> { "" }.Concat(_loadedModel.AvailableLots).ToList(),
            ColorList = new List<string> { "" }.Concat(_loadedModel.AvailableColors).ToList(),
            UnitList = new List<string> { "" }.Concat(_loadedModel.AvailableUnits).ToList()

        };

        OnPropertyChanged(nameof(DraftRow));
        OnPropertyChanged(nameof(IsDraftOpen));
    }

    string ExtractCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        var idx = value.IndexOf('-');
        return idx > 0 ? value.Substring(0, idx).Trim() : value;
    }



    public void ConfirmDraft()
    {
        if (DraftRow == null || Original == null)
            return;

        if (DraftRow.Quantity <= 0)
            return;
    
        // 🔴 ORİJİNALLE AYNI MI KONTROLÜ
        bool sameAsOriginal =
            DraftRow.LotCode == Original.LotCode &&
            DraftRow.ColorCode == Original.ColorCode &&
            DraftRow.UnitCode == Original.UnitCode &&
            DraftRow.Quantity == Original.Quantity;

        if (sameAsOriginal)
            return; // ❌ birebir aynısını ekleme

        // 🔴 CLONE OLUŞTUR (referans bug’ını da önler)
        var newRow = new SplitRowVm
        {
            IsExisting = false,
            ItemCode = DraftRow.ItemCode,
            ItemName = DraftRow.ItemName,
            LotCode = DraftRow.LotCode,
            ColorCode = ExtractCode(DraftRow.ColorCode),
            UnitCode = DraftRow.UnitCode,
            Quantity = DraftRow.Quantity
        };

        NewSplits.Add(newRow);

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
    // EDIT / DELETE
    // ===============================
    public void Edit(SplitRowVm row)
    {
        if (row.IsExisting)
            return;

        row.IsEditing = true;
    }

    public void FinishEdit(SplitRowVm row)
    {
        row.IsEditing = false;
    }

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
        var draft = SplitMapper.ToErpDraft(
            OriginalBarcode,
            NewSplits);

        draft.OriginalBarcode = Original.Barcode;


        // 🔴 EN BAŞTA: her şeyi kapat
        HasOriginalBarcode = false;
        Original = null;
        _loadedModel = null;

        ExistingSplits.Clear();
        NewSplits.Clear();

        OnPropertyChanged(nameof(Original));
        OnPropertyChanged(nameof(OriginalLine2));
        OnPropertyChanged(nameof(MixedItems));

        SplitBarcodeModel model;

        try
        {
            model = await _service.SaveAsync(draft);
        }
        catch
        {
            // barkod bulunamadı → UI kapalı kalır
            return;
        }

        if (model == null)
            return;

        // ✅ SADECE BAŞARILIYSA
        _loadedModel = model;
        Original = SplitMapper.ToVm(model, true);
        HasOriginalBarcode = true;

        OnPropertyChanged(nameof(Original));
        OnPropertyChanged(nameof(OriginalLine2));

        foreach (var e in model.ExistingSplits)
            ExistingSplits.Add(SplitMapper.ToVm(e, true));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
