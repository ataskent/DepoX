
namespace DepoX.Services.Erp.Dtos;

public class TransferList
{
    public List<TransferM> Transfers { get; set; } = new();
    public List<Whouse> Whouses { get; set; } = new();
}

public class TransferData
{
    public TransferM TransferM { get; set; } = new();
    public List<Barcode> Barcodes { get; set; } = new();
    public List<Items> Items { get; set; } = new();
}

public class Whouse
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public class TransferM
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string WhouseCode { get; set; } = "";
}

public class Barcode
{
    public string WhouseCode { get; set; } = "";
    public string Code { get; set; } = "";
    public decimal Quantity { get; set; }
}

public class Items
{
    public string WhouseCode { get; set; } = "";
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Quantity { get; set; }
}