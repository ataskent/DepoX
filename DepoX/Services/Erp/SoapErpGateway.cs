using DepoX.Services.Erp;
using System.Threading;
using System.Threading.Tasks;

namespace DepoX.Services.Erp
{
    public class SoapErpGateway : IErpGateway
    {
        public Task<ErpResult> CommitAsync(Models.StockTransaction transaction, CancellationToken cancellationToken = default)
        {
            var result = ErpResult.Failed("SOAP ERP gateway not implemented yet.");
            return Task.FromResult(result);
        }
    }
}
