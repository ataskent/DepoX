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
