using DepoX.Services.Erp.Dtos;

namespace DepoX.Features.Shared;

public static class SharedMapper
{
    public static SharedDraft ToModel(this SharedViewModel vm)
    {
        return new SharedDraft
        {
            ClientDraftId = vm.ClientDraftId,
            CreatedAt = vm.CreatedAt,
            Items = vm.Items.Select(x => new SharedItem
            {
                Barcode = x.Barcode,
                Quantity = x.Quantity
            }).ToList()
        };
    }

    public static WhouseItemVm ToModel(this ErpWhouseDto dto)
    {
        return new WhouseItemVm
        {
            Code = dto.Code,
            Name = dto.Name
        };
    }
    public static BranchItemVm ToModel(this ErpBranchDto dto)
    {
        return new BranchItemVm
        {
            Code = dto.Code,
            Name = dto.Name
        };
    }
}

public static class SharedErpMapper
{
    public static ErpBasketDraft ToErp(this SharedDraft draft)
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

