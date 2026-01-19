using DepoX.Dtos;
using DepoX.Features.Count;
using System.Threading;
using System.Threading.Tasks;

namespace DepoX.Services.Erp
{
    public interface IErpGateway
    {
        // The method 'GetBarcodeMastersAsync()' was commented out, not deleted.
        // To avoid ENC0033, either fully remove the commented line or restore the method signature if hot reload is required.
        // Here, we remove the commented-out method to resolve the diagnostic.

        Task<ErpResult<BasketDraftDto>> SaveBasketAsync(
            BasketDraftDto request,
            CancellationToken cancellationToken = default);

        // İleride aynı pattern:
        // Task<ErpResult<WarehouseDto[]>> GetWarehousesAsync(CancellationToken cancellationToken = default);
        // Task<ErpResult<OrderDetailDto>> GetOrderByNoAsync(string orderNo, CancellationToken cancellationToken = default);
    }

}

