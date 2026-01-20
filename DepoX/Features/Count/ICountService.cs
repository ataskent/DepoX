using DepoX.Dtos;
using DepoX.Features.Count;
using DepoX.Services.Erp.Dtos;

public interface ICountService
{
    Task<ErpResult<ErpBasketDraft>> SaveAsync(
        CountDraft draft,
        CancellationToken cancellationToken = default);
}
