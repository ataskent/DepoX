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


    public bool IsInvalid { get; set; }
}

public class ValidatedStockVm
{
    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";
    public decimal Quantity { get; set; }
    public bool HasError { get; set; }
}