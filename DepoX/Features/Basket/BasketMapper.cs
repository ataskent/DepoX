using DepoX.Services.Erp.Dtos;

namespace DepoX.Features.Basket;

public static class BasketMapper
{
    public static BasketDraft ToModel(this BasketViewModel vm)
    {
        return new BasketDraft
        {
            BasketNo = vm.BasketNo ?? string.Empty,
            WhouseCode = vm.SelectedWhouse?.Code ?? string.Empty,
            ClientDraftId = vm.ClientDraftId,
            CreatedAt = vm.CreatedAt,
            Items = vm.Items.Select(x => new BasketItem
            {
                Barcode = x.Barcode,
                Quantity = x.Quantity
            }).ToList()
        };
    }

    public static BasketVm ToModel(this ErpBasketDto dto)
    {
        return new BasketVm
        {
            Code = dto.Code,
            Name = dto.Code
        };
    }
    public static WhouseVm ToModel(this ErpWhouseDto dto)
    {
        return new WhouseVm 
        {
            Code = dto.Code,
            Name = dto.Name
        };
    }
}

public static class BasketErpMapper
{
    public static ErpBasketDraft ToErp(this BasketDraft draft)
    {
        return new ErpBasketDraft
        {
            BasketNo = draft.BasketNo,
            WhouseCode = draft.WhouseCode,
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
