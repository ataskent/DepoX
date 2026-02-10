namespace DepoX.Features.FromWorder;


public class TransferDataVm
{
    public TransferMVm Transfer { get; set; } = new();
    public List<BarcodeVm> Barcodes { get; set; } = new();
    public List<ItemsVm> Items { get; set; } = new();
}

public class WhouseVm
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public class TransferMVm
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string WhouseCode { get; set; } = "";
}

public class BarcodeVm
{
    public string WhouseCode { get; set; } = "";
    public string Code { get; set; } = "";
    public decimal Quantity { get; set; }
    public string LotCode { get; set; } = "";
    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public string ColorCode { get; set; } = "";

    public string LineA => $"Parti: {LotCode} - Renk: {ColorCode}";
    public string LineB => $"Miktar: {Quantity} {UnitCode} - Depo: {WhouseCode}";
}

public class ItemsVm
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal Qty { get; set; }
    public string WhouseCode { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public string LineA => $"Miktar: {Quantity} {UnitCode} - İşlenen Miktar: {Qty} {UnitCode}";
}
