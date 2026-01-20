namespace DepoX.Features.Count;

public static class CountMappings
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
