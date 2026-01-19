namespace DepoX.Features.Count;

/* =========================
   UI / FEATURE DTO’LARI
   ========================= */

public class CountDraftDto
{
    public Guid ClientDraftId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CountItemDto> Items { get; set; } = new();
}

public class CountItemDto
{
    public string Barcode { get; set; } = default!;
    public decimal Quantity { get; set; }
}


