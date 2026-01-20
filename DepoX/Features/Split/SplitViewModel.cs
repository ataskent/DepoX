using DepoX.Services.Erp.Dtos;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DepoX.Features.Split;

public class SplitViewModel : INotifyPropertyChanged
{
    // ===== Original Barcode =====
    public SplitRowVm? Original { get; private set; }

    public bool HasOriginalBarcode => Original != null;

    public string OriginalLine2 =>
        Original == null
            ? ""
            : $"{Original.LotCode} · {Original.ColorCode} · {Original.UnitCode} · Miktar: {Original.Quantity}";

    // ===== Lists =====
    public ObservableCollection<SplitRowVm> NewSplits { get; } = new();
    public ObservableCollection<SplitRowVm> ExistingSplits { get; } = new();
    public ObservableCollection<SplitRowVm> MixedItems { get; } = new();

    public int TotalBarcodeCount => NewSplits.Count + ExistingSplits.Count;

    // ===== PUBLIC API =====

    public void SetOriginal(SplitRowVm original)
    {
        Original = original;
        OnPropertyChanged(nameof(Original));
        OnPropertyChanged(nameof(HasOriginalBarcode));
        OnPropertyChanged(nameof(OriginalLine2));
    }

    public void AddNewSplit()
    {
        CloseAllEdits();

        var row = new SplitRowVm
        {
            IsExisting = false,
            IsEditing = true,
            ItemCode = Original?.ItemCode ?? "",
            ItemName = Original?.ItemName ?? "",
            LotCode = Original?.LotCode ?? "",
            ColorCode = Original?.ColorCode ?? "",
            UnitCode = Original?.UnitCode ?? "",
            Quantity = 0,

            // şimdilik dummy – ERP’den doldurulacak
            StockList = new[] { "STK-001", "STK-002" },
            LotList = new[] { "P01", "P02" },
            ColorList = new[] { "KIRMIZI", "MAVİ" },
            UnitList = new[] { "ADET", "KG" }
        };

        NewSplits.Insert(0, row);
        RebuildMixedItems();
    }

    public void Edit(SplitRowVm row)
    {
        CloseAllEdits();
        row.IsEditing = true;
    }

    public void FinishEdit(SplitRowVm row)
    {
        row.IsEditing = false;
        RebuildMixedItems();
    }

    public void Remove(SplitRowVm row)
    {
        if (row.IsExisting)
            ExistingSplits.Remove(row);
        else
            NewSplits.Remove(row);

        RebuildMixedItems();
    }

    // ===== Helpers =====

    void CloseAllEdits()
    {
        foreach (var r in MixedItems)
            r.IsEditing = false;
    }

    void RebuildMixedItems()
    {
        MixedItems.Clear();

        foreach (var n in NewSplits)
            MixedItems.Add(n);

        foreach (var e in ExistingSplits)
            MixedItems.Add(e);

        OnPropertyChanged(nameof(TotalBarcodeCount));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public async Task LoadFromErpAsync(
    ErpBarcodeDetailDto dto)
    {
        // 1) Original
        SetOriginal(new SplitRowVm
        {
            IsExisting = false,
            ItemCode = dto.ItemCode,
            ItemName = dto.ItemName,
            LotCode = dto.LotCode,
            ColorCode = dto.ColorCode,
            UnitCode = dto.UnitCode,
            Quantity = dto.Quantity
        });

        // 2) Eski splitler
        ExistingSplits.Clear();

        foreach (var s in dto.ExistingSplits)
        {
            ExistingSplits.Add(new SplitRowVm
            {
                IsExisting = true,
                ItemCode = s.ItemCode,
                ItemName = s.ItemName,
                LotCode = s.LotCode,
                ColorCode = s.ColorCode,
                UnitCode = s.UnitCode,
                Quantity = s.Quantity
            });
        }

        NewSplits.Clear();
        RebuildMixedItems();
    }

}

public class SplitRowVm : INotifyPropertyChanged
{
    // ===== State =====
    public bool IsExisting { get; set; }

    bool _isEditing;
    public bool IsEditing
    {
        get => _isEditing;
        set
        {
            if (_isEditing == value) return;
            _isEditing = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNewAndNotEditing));
        }
    }

    public bool IsNewAndNotEditing => !IsExisting && !IsEditing;

    // ===== Data =====
    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";  
    public string LotCode { get; set; } = "";
    public string ColorCode { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public decimal Quantity { get; set; }

    // ===== ERP Seçim Listeleri =====
    public IList<string> StockList { get; set; } = new List<string>();
    public IList<string> LotList { get; set; } = new List<string>();
    public IList<string> ColorList { get; set; } = new List<string>();
    public IList<string> UnitList { get; set; } = new List<string>();

    // ===== Liste görünümü =====
    public string SummaryLine =>
        $"{ItemCode} · {ItemName} · {LotCode} · {ColorCode} · {UnitCode} · {Quantity}";

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}


