namespace DepoX.Dtos;

/// <summary>
/// Tek bir barkod satırı (UI modelinden KOPUK).
/// </summary>
public class CountDraftItemDto
{
    public string Barcode { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    // Şimdilik boş ama sözleşmede dursun
    public string StockCode { get; set; } = string.Empty;
    public string FromWarehouseCode { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
}
