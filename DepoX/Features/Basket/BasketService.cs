using DepoX.Dtos;
using DepoX.Services.Erp;
using DepoX.Services.Erp.Dtos;
using Microsoft.Maui.Animations;

namespace DepoX.Features.Basket;

public class BasketService : IBasketService
{
    private readonly IErpGateway _erpGateway;

    public BasketService(IErpGateway erpGateway)
    {
        _erpGateway = erpGateway;
    }

    public Task<ErpResult<ErpBasketDraft>> SaveAsync(
        BasketDraft draft,
        CancellationToken cancellationToken = default)
    {
        var erpDraft = draft.ToErp();
        return _erpGateway.SaveBasketAsync(erpDraft, cancellationToken);
    }

   

    public Task<ErpResult<ErpBasketWhouseDto>> GetBasketDataAsync(CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.GetBasketDataAsync(cancellationToken);
        return result;
        
    }

    public Task<ErpResult<List<BasketItemDto>>> GetBasketBarcodeDataAsync(string basketCode, CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.GetBasketBarcodeDataAsync(basketCode, cancellationToken);
        return result;

    }

    public Task<ErpResult<bool>> ClearBasketDataAsync(string basketCode, string whouseCode, CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.ClearBasketDataAsync(basketCode, whouseCode, cancellationToken);
        return result;

    }
    public Task<ErpResult<bool>> DeleteBasketDataAsync(string basketCode, CancellationToken cancellationToken = default)
    {
        var result = _erpGateway.DeleteBasketDataAsync(basketCode, cancellationToken);
        return result;

    }

}

public interface IBasketService
{
        Task<ErpResult<ErpBasketDraft>> SaveAsync(
        BasketDraft draft,
        CancellationToken cancellationToken = default);

        Task<ErpResult<ErpBasketWhouseDto>> GetBasketDataAsync(
        CancellationToken cancellationToken = default);

    Task<ErpResult<List<BasketItemDto>>> GetBasketBarcodeDataAsync(string basketCode, CancellationToken cancellationToken = default);
    Task<ErpResult<bool>> ClearBasketDataAsync(string basketCode, string whouseCode, CancellationToken cancellationToken = default);
    Task<ErpResult<bool>> DeleteBasketDataAsync(string basketCode, CancellationToken cancellationToken = default);

}
