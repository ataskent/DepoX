using DepoX.Services.Erp;

namespace DepoX.Features.Count;

public class CountService : ICountService
{
    private readonly IErpGateway _erpGateway;

    public CountService(IErpGateway erpGateway)
    {
        _erpGateway = erpGateway;
    }

    public async Task SaveAsync(
        CountDraftDto draft,
        CancellationToken cancellationToken = default)
    {
        if (draft.Items.Count == 0)
            throw new InvalidOperationException("Gönderilecek barkod yok.");

        // ?? UI ? ERP dönüþümü
        var erpRequest = MapToErpBasket(draft);

        var result = await _erpGateway.SaveBasketAsync(
            erpRequest,
            cancellationToken);

        if (!result.Success)
            throw new Exception(result.Message ?? "ERP kayýt hatasý.");
    }

    private static ErpBasketDraft MapToErpBasket(CountDraftDto draft)
    {
        return new ErpBasketDraft
        {
            ClientDraftId = draft.ClientDraftId,
            CreatedAt = draft.CreatedAt,
            Items = draft.Items.Select(x => new ErpBasketItem
            {
                Barcode = x.Barcode,
                Quantity = x.Quantity
            }).ToArray()
        };
    }
}
