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


    public Task<ErpResult<TransferList>> GetTransferAsync(CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.GetTransferAsync(cancellationToken);
        return result;

    }

    public Task<ErpResult<TransferData>> GetTransferDataAsync(string transferCode,CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.GetTransferDataAsync(transferCode, cancellationToken);
        return result;

    }

    public Task<ErpResult<TransferData>> SaveTransferAsync(TransferData transferData, CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.SaveTransferAsync(transferData, cancellationToken);
        return result;

    }

}

public interface IFromWorderService
{
    Task<ErpResult<TransferList>> GetTransferAsync(
    CancellationToken cancellationToken = default);


    Task<ErpResult<TransferData>> GetTransferDataAsync(string transferCode,
    CancellationToken cancellationToken = default);
    Task<ErpResult<TransferData>> SaveTransferAsync(TransferData transferData,
    CancellationToken cancellationToken = default);


}
