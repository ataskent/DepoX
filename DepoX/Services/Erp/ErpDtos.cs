namespace DepoX.Services.Erp;

public class ErpBasketDraft
{
    public Guid ClientDraftId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ErpBasketItem[] Items { get; set; } = Array.Empty<ErpBasketItem>();
}

public class ErpBasketItem
{
    public string Barcode { get; set; } = default!;
    public decimal Quantity { get; set; }
}
