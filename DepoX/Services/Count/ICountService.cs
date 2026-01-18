using DepoX.Dtos;

namespace DepoX.Services.Count;

public interface ICountService
{
    /// <summary>
    /// Terminalde okutulan tüm barkodları tek seferde kaydeder
    /// </summary>
    Task SaveDraftAsync(
        CountDraftDto draft,
        CancellationToken cancellationToken = default);
}
