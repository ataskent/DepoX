using DepoX.Services.Erp.Dtos;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DepoX.Features.Split;

public class SplitRowVm : INotifyPropertyChanged
{
    public bool IsNewBarcodeMode { get; set; }

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

    string _newBarcode = "";
    public string NewBarcode
    {
        get => _newBarcode;
        set
        {
            _newBarcode = value;
            OnPropertyChanged();
        }
    }
    public bool HasNewBarcode =>
        !string.IsNullOrWhiteSpace(NewBarcode);

    public ICommand ClearLotCommand => new Command(() => LotCode = "");
    public ICommand ClearColorCommand => new Command(() => ColorCode = "");
    public ICommand ClearUnitCommand => new Command(() => UnitCode = "");

    string _itemSearchText = "";

    public string ItemSearchText
    {
        get => _itemSearchText;
        set
        {
            _itemSearchText = value;
            FilterItems();
            OnPropertyChanged();
        }
    }

    public string ItemDisplay =>
    SelectedItem == null ? "" : $"{SelectedItem.Name}";


    public List<ItemMetaDto> ItemList { get; set; } = new();
    public ObservableCollection<ItemMetaDto> FilteredItemList { get; } = new();

    ItemMetaDto? _selectedItem;
    public ItemMetaDto? SelectedItem
    {
        get => _selectedItem;
        set
        {
            _selectedItem = value;
            ItemCode = value?.Code ?? "";

            OnPropertyChanged(); // SelectedItem
            OnPropertyChanged(nameof(ItemDisplay)); // 🔥 BUNU EKLE
        }
    }


    void FilterItems()
    {
        FilteredItemList.Clear();

        IEnumerable<ItemMetaDto> query =
            string.IsNullOrWhiteSpace(ItemSearchText)
                ? ItemList
                : ItemList.Where(x =>
                    x.Code.Contains(ItemSearchText, StringComparison.OrdinalIgnoreCase) ||
                    x.Name.Contains(ItemSearchText, StringComparison.OrdinalIgnoreCase));

        // 🔥 Performans için sınır
        foreach (var item in query.Take(50))
            FilteredItemList.Add(item);
    }



    // ✅ XAML BUNU KULLANIYOR
    public bool IsNewAndNotEditing => !IsExisting && !IsEditing;

    string _itemCode = "";
    public string ItemCode
    {
        get => _itemCode;
        set
        {
            if (_itemCode == value) return;
            _itemCode = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(SummaryLine));

            // 🔥 KRİTİK
            OnItemChanged?.Invoke(_itemCode);
        }
    }

    string _itemName = "";
    public string ItemName
    {
        get => _itemName;
        set { if (_itemName == value) return; _itemName = value; OnPropertyChanged(); OnPropertyChanged(nameof(SummaryLine)); }
    }

    string _lotCode = "";
    public string LotCode
    {
        get => _lotCode;
        set { if (_lotCode == value) return; _lotCode = value; OnPropertyChanged(); OnPropertyChanged(nameof(SummaryLine)); }
    }

    string _colorCode = "";
    public string ColorCode
    {
        get => _colorCode;
        set
        {
            if (_colorCode == value) return;
            _colorCode = value;
            OnPropertyChanged(); OnPropertyChanged(nameof(SummaryLine));
        }
    }

    string _barcode = "";
    public string Barcode
    {
        get => _barcode;
        set
        {
            if (_barcode == value) return;
            _barcode = value;
            OnPropertyChanged(); OnPropertyChanged(nameof(SummaryLine));
        }
    }



    string _unitCode = "";
    public string UnitCode
    {
        get => _unitCode;
        set { if (_unitCode == value) return; _unitCode = value; OnPropertyChanged(); OnPropertyChanged(nameof(SummaryLine)); }
    }

    decimal _quantity;
    public decimal Quantity
    {
        get => _quantity;
        set { if (_quantity == value) return; _quantity = value; OnPropertyChanged(); OnPropertyChanged(nameof(SummaryLine)); }
    }

    // seçim listeleri
    //public IList<string> ItemList { get; set; } = new List<string>();
    public IList<string> LotList { get; set; } = new List<string>();
    public IList<string> ColorList { get; set; } = new List<string>();
    public IList<string> UnitList { get; set; } = new List<string>();


    // ✅ XAML BUNU KULLANIYOR
    public string SummaryLine =>
        $"Parti: {LotCode}  - Renk : {ColorCode}  - Miktar : {Quantity} {UnitCode}";

    public event PropertyChangedEventHandler? PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public Action<string>? OnItemChanged { get; set; }
}
