using DepoX.Services.Erp;
using DepoX.Services.Erp.Dtos;

namespace DepoX.Features.Count;

public static class CountErpMappings
{
    public static ErpBasketDraft ToErp(this CountDraft draft)
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
