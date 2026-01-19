using DepoX.Dtos;
using DepoX.Services.Erp;
using System.Collections.ObjectModel;

namespace DepoX.Features.Count;

public class CountService : ICountService
{
    private readonly IErpGateway _erpGateway;

    public CountService(IErpGateway erpGateway)
    {
        _erpGateway = erpGateway;
    }

    // UI SEPETİ: CountDraftItemDto
    public void AddByBarcode(
        ObservableCollection<CountItemModel> basket,
        string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return;

        var existing = basket.FirstOrDefault(x => x.Barcode == barcode);

        if (existing != null)
        {
            existing.Quantity += 1;
            return;
        }

        basket.Add(new CountItemModel
        {
            Barcode = barcode,
            Quantity = 1
        });
    }

    public void RemoveByBarcode(
        ObservableCollection<CountItemModel> basket,
        CountItemModel item)
    {
        if (item == null)
            return;

        var existing = basket.FirstOrDefault(x => x == item);

        if (existing != null)
        {
            if (existing.Quantity > 1)
            {
                existing.Quantity -= 1;
            }
            else
            {
                basket.Remove(existing);
            }
        }
    }

    
    public async Task SaveDraftAsync(
        CountDraftDto draft,
        CancellationToken cancellationToken = default)
    {
        if (draft.Items == null || draft.Items.Count == 0)
            throw new InvalidOperationException("Gönderilecek barkod yok.");

        var erpRequest = new BasketDraftDto
        {
            ClientDraftId = draft.ClientDraftId,
            WorkplaceCode = draft.WorkplaceCode,
            Items = draft.Items.Select(x => new BasketItemDraftDto
            {
                Barcode = x.Barcode,
                Quantity = x.Quantity
            }).ToArray()
        };

        var result = await _erpGateway.SaveBasketAsync(
            erpRequest,
            cancellationToken);

        if (!result.Success)
            throw new Exception(result.Message ?? "ERP kayıt hatası.");
    }
}
