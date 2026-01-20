using DepoX.Services.Erp.Dtos;

namespace DepoX.Features.Count;

public static class CountMapper
{
    public static CountDraft ToModel(this CountViewModel vm)
    {
        return new CountDraft
        {
            ClientDraftId = vm.ClientDraftId,
            CreatedAt = vm.CreatedAt,
            Items = vm.Items.Select(x => new CountItem
            {
                Barcode = x.Barcode,
                Quantity = x.Quantity
            }).ToList()
        };
    }
}

public static class CountErpMapper
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

