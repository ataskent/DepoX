namespace DepoX.Dtos;

public class CountItem
{
    public string Barcode { get; set; } = null!;

    public string ItemCode { get; set; } = null!;
    public string? LotCode { get; set; }
    public string? ColorCode { get; set; }
    public string UnitCode { get; set; } = null!;

    public decimal Quantity { get; set; }
}
