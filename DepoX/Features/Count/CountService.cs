using DepoX.Dtos;
using DepoX.Features.Count;
using DepoX.Services.Erp;
using DepoX.Services.Erp.Dtos;

namespace DepoX.Features.Count;
public class CountService : ICountService
{
    private readonly IErpGateway _erpGateway;

    public CountService(IErpGateway erpGateway)
    {
        _erpGateway = erpGateway;
    }

    public Task<ErpResult<CountM>> SaveAsync(
        CountM countM,
        CancellationToken cancellationToken = default)
    {
        return _erpGateway.SaveCountAsync(countM, cancellationToken);
    }

    public Task<ErpResult<CountList>> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.GetCountAsync(cancellationToken);
        return result;

    }

    public Task<ErpResult<CountM>> GetCountDataAsync(string countCode, CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.GetCountDataAsync(countCode, cancellationToken);
        return result;

    }
    public Task<ErpResult<bool>> DeleteCountDataAsync(string countCode, CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.DeleteCountDataAsync(countCode, cancellationToken);
        return result;

    }
}

public interface ICountService
{
    Task<ErpResult<CountM>> SaveAsync(
        CountM countM,
        CancellationToken cancellationToken = default);

    Task<ErpResult<CountList>> GetCountAsync(
    CancellationToken cancellationToken = default);

    Task<ErpResult<CountM>> GetCountDataAsync(string countCode,
        CancellationToken cancellationToken = default);

    Task<ErpResult<bool>> DeleteCountDataAsync(string countCode,
        CancellationToken cancellationToken = default);
}

