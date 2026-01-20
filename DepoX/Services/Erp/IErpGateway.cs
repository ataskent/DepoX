using DepoX.Dtos;
using DepoX.Services.Erp.Dtos;

namespace DepoX.Services.Erp
{
    public interface IErpGateway
    {
        Task<ErpResult<ErpBasketDraft>> SaveBasketAsync(
            ErpBasketDraft draft,
            CancellationToken cancellationToken = default);
    }

}

