using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DepoX.Features.Split;

public class SplitRowVm : INotifyPropertyChanged
{
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
   

    public ICommand ClearLotCommand => new Command(() => LotCode = "");
    public ICommand ClearColorCommand => new Command(() => ColorCode = "");
    public ICommand ClearUnitCommand => new Command(() => UnitCode = "");


    // ✅ XAML BUNU KULLANIYOR
    public bool IsNewAndNotEditing => !IsExisting && !IsEditing;

    string _itemCode = "";
    public string ItemCode
    {
        get => _itemCode;
        set { if (_itemCode == value) return; _itemCode = value; OnPropertyChanged(); OnPropertyChanged(nameof(SummaryLine)); }
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
    public IList<string> StockList { get; set; } = new List<string>();
    public IList<string> LotList { get; set; } = new List<string>();
    public IList<string> ColorList { get; set; } = new List<string>();
    public IList<string> UnitList { get; set; } = new List<string>();

    // ✅ XAML BUNU KULLANIYOR
    public string SummaryLine =>
        $"Parti: {LotCode}  - Renk : {ColorCode}  - Miktar : {Quantity} {UnitCode}";

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
