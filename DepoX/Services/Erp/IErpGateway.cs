using DepoX.Dtos;
using DepoX.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DepoX.Services.Erp
{
    public interface IErpGateway
    {
        Task<ErpResult<BasketSnapshotDto>> SaveBasketAsync(
            BasketDraftDto request,
            CancellationToken cancellationToken = default);

        // İleride aynı pattern:
        // Task<ErpResult<WarehouseDto[]>> GetWarehousesAsync(CancellationToken cancellationToken = default);
        // Task<ErpResult<OrderDetailDto>> GetOrderByNoAsync(string orderNo, CancellationToken cancellationToken = default);
    }

    public interface IErpCommitGateway
    {
        Task<ErpResult> CommitAsync(
            StockTransaction transaction,
            CancellationToken cancellationToken = default);
    }
}

