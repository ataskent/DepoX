using DepoX.Dtos;
using DepoX.Features.Count;
using DepoX.Services.Erp;
using DepoX.Services.Erp.Dtos;

public class CountService : ICountService
{
    private readonly IErpGateway _erpGateway;

    public CountService(IErpGateway erpGateway)
    {
        _erpGateway = erpGateway;
    }

    public Task<ErpResult<ErpBasketDraft>> SaveAsync(
        CountDraft draft,
        CancellationToken cancellationToken = default)
    {
        var erpDraft = draft.ToErp();
        return _erpGateway.SaveBasketAsync(erpDraft, cancellationToken);
    }
}
