namespace DepoX.Features.Shared;

public class SharedDraft
{
    public Guid ClientDraftId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<SharedItem> Items { get; set; } = new();
}

public class SharedItem
{
    public string Barcode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class WhouseItemVm
{
    public string BranchCode { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class BranchItemVm
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}