namespace DepoX.Features.Count;

public class CountDraft
{
    public Guid ClientDraftId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CountItem> Items { get; set; } = new();
}

public class CountItem
{
    public string Barcode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}
