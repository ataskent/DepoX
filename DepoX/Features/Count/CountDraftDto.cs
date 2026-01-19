namespace DepoX.Features.Count;

/// <summary>
/// Terminalden servise giden sayım taslağı.
/// Tek "Kaydet" ile gönderilir.
/// </summary>
public class CountDraftDto
{
    /// <summary>
    /// Terminal tarafında üretilen geçici ID.
    /// Offline / retry senaryoları için.
    /// </summary>
    public string ClientDraftId { get; set; } = string.Empty;

    /// <summary>
    /// İşyeri / tesis kodu
    /// </summary>
    public string WorkplaceCode { get; set; } = string.Empty;

    /// <summary>
    /// Sayım satırları
    /// </summary>
    public List<CountDraftItemDto> Items { get; set; } = new();
}
