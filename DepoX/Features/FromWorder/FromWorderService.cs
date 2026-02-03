using DepoX.Dtos;
using DepoX.Features.Basket;
using DepoX.Services.Erp;
using DepoX.Services.Erp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Features.FromWorder;

public class FromWorderService : IFromWorderService
{
    private readonly IErpGateway _erpGateway;

    public FromWorderService(IErpGateway erpGateway)
    {
        _erpGateway = erpGateway;
    }


    public Task<ErpResult<ErpBasketWhouseDto>> GetBasketDataAsync(CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.GetBasketDataAsync(cancellationToken);
        return result;

    }

}

public interface IFromWorderService
{
    Task<ErpResult<ErpBasketWhouseDto>> GetBasketDataAsync(
    CancellationToken cancellationToken = default);


}
