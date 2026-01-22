using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DepoX.Features.Split;

public class SplitBarcodeModel
{
    public string Barcode { get; set; } = "";
    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";
    public string LotCode { get; set; } = "";
    public string ColorCode { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public decimal Quantity { get; set; }

    public List<string> AvailableLots { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
    public List<string> AvailableUnits { get; set; } = new();

    public List<SplitBarcodeModel> ExistingSplits { get; set; } = new();
}
