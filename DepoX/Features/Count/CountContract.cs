namespace DepoX.Features.Count;

public interface ICountService
{
    Task SaveAsync(
        CountDraftDto draft,
        CancellationToken cancellationToken = default);
}
