using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DepoX.Features.Basket;

public class BasketDraft
{
    public string BasketNo { get; set; } = string.Empty;
    public string WhouseCode { get; set; } = string.Empty;
    public Guid ClientDraftId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<BasketItem> Items { get; set; } = new();
}

public class BasketItem
{
    public string WhouseCode { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}


public class BasketData
{
    public List<WhouseVm> Whouses { get; set; } = new();
    public List<BasketVm> Baskets { get; set; } = new();
}

public class WhouseVm
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public class BasketVm
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public class BasketItemVm : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    string _barcode = "";
    public string Barcode
    {
        get => _barcode;
        set { _barcode = value; OnPropertyChanged(); }
    }

    decimal _qty;
    public decimal Qty
    {
        get => _qty;
        set { _qty = value; OnPropertyChanged(); }
    }

    decimal _quantity;
    public decimal Quantity
    {
        get => _quantity;
        set { _quantity = value; OnPropertyChanged(); }
    }

    string _whouse = "";
    public string Whouse
    {
        get => _whouse;
        set { _whouse = value; OnPropertyChanged(); }
    }

    string _itemCode = "";
    public string ItemCode
    {
        get => _itemCode;
        set { _itemCode = value; OnPropertyChanged(); }
    }

    string _itemName = "";
    public string ItemName
    {
        get => _itemName;
        set { _itemName = value; OnPropertyChanged(); }
    }

    string _lotCode = "";
    public string LotCode
    {
        get => _lotCode;
        set { _lotCode = value; OnPropertyChanged(); }
    }

    string colorCode = "";
    public string ColorCode
    {
        get => colorCode;
        set { colorCode = value; OnPropertyChanged(); }
    }

    string _unitCode = "";
    public string UnitCode
    {
        get => _unitCode;
        set { _unitCode = value; OnPropertyChanged(); }
    }

    string _lineA = "";
    public string LineA
    {
        get =>  $"Parti : {LotCode} - Renk : {ColorCode}";
        set { _lineA = value; OnPropertyChanged(); }
    }

    string _lineB = "";
    public string LineB
    {
        get => $"Miktar : {Qty} {UnitCode}";
        set { _lineB = value; OnPropertyChanged(); }
    }

  


    public bool IsInvalid { get; set; }
}

public class ValidatedStockVm
{
    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";
    public decimal Quantity { get; set; }
    public bool HasError { get; set; }

    public string LotCode { get; set; } = "";
    public string ColorCode { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public decimal Qty { get; set; }

    public string LineA
    {
        get => $"Miktar : {Qty} {UnitCode}";
    }


}