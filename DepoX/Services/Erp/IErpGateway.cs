using System.Threading;
using System.Threading.Tasks;

namespace DepoX.Services.Erp
{
    public interface IErpGateway
    {
        Task<ErpResult> CommitAsync(Models.StockTransaction transaction, CancellationToken cancellationToken = default);
    }
}
