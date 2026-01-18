using DepoX.Dtos;
using DepoX.Services.Erp;

namespace DepoX.Services.Count;

public class CountService : ICountService
{
    private readonly IErpGateway _erpGateway;

    public CountService(IErpGateway erpGateway)
    {
        _erpGateway = erpGateway;
    }

    public async Task SaveDraftAsync(
        CountDraftDto draft,
        CancellationToken cancellationToken = default)
    {
        if (draft == null)
            throw new ArgumentNullException(nameof(draft));

        if (draft.Items == null || draft.Items.Count == 0)
            throw new InvalidOperationException("Gönderilecek sayım satırı yok.");

        // ✅ ERP DTO — birebir sözleşme
        var erpRequest = new BasketDraftDto
        {
            ClientDraftId = draft.ClientDraftId,
            BasketId = "", // yeni sepet
            WorkplaceCode = draft.WorkplaceCode,
            Items = draft.Items.Select(x => new BasketItemDraftDto
            {
                Barcode = x.Barcode,
                StockCode = x.StockCode,
                Quantity = x.Quantity,
                FromWarehouseCode = x.FromWarehouseCode,
                LocationCode = x.LocationCode
            }).ToArray()
        };

        // 🔑 ERP entegrasyonu (generic + tip güvenli)
        ErpResult<BasketSnapshotDto> result =
            await _erpGateway.SaveBasketAsync(
                erpRequest,
                cancellationToken);

        if (!result.Success)
            throw new Exception(result.Message ?? "ERP kayıt hatası.");

        // 🔥 BURASI ÇOK KIYMETLİ
        // ERP'den gelen BasketId artık sende
        var snapshot = result.Data;

        // İstersen burada:
        // - local basket güncelle
        // - snapshot.BasketId sakla
        // - status = Open yap
    }
}
