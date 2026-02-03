using DepoX.Dtos;
using DepoX.Services.Erp;
using DepoX.Services.Erp.Dtos;

namespace DepoX.Features.Shared;
public class SharedService : ISharedService
{
    private readonly IErpGateway _erpGateway;

    public SharedService(IErpGateway erpGateway)
    {
        _erpGateway = erpGateway;
    }

    public Task<ErpResult<ErpOptionalDto>> GetOptionalDataAsync(CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.GetOptionalDataAsync(cancellationToken);
        return result;

    }
}

public interface ISharedService
{
    Task<ErpResult<ErpOptionalDto>> GetOptionalDataAsync(
        CancellationToken cancellationToken = default);
}

