using DepoX.Services.Erp;
using DepoX.Services.Erp.Dtos;

namespace DepoX.Features.Split;

public class SplitService : ISplitService
{
    private readonly IErpGateway _erp;

    public SplitService(IErpGateway erp)
    {
        _erp = erp;
    }

    public async Task<ServiceResult<SplitBarcodeModel>> GetBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return ServiceResult<SplitBarcodeModel>.Fail("Barkod boş olamaz.");

        try
        {
            var result = await _erp.GetBarcodeDetailAsync(barcode.Trim(), cancellationToken);

            if (!result.Success || result.Data == null)
                return ServiceResult<SplitBarcodeModel>.Fail(
                    result.Message ?? "ERP barkod bilgisi alınamadı.");

            var model = SplitMapper.ToModel(result.Data);
            return ServiceResult<SplitBarcodeModel>.Ok(model);
        }
        catch (OperationCanceledException)
        {
            return ServiceResult<SplitBarcodeModel>.Fail("İşlem iptal edildi.", "CANCELED");
        }
        catch (Exception ex)
        {
            // İstersen buraya log da ekleriz (ILogger vs)
            return ServiceResult<SplitBarcodeModel>.Fail(ex.Message, "UNHANDLED");
        }
    }

    public async Task<ServiceResult<SplitBarcodeModel>> SaveAsync(
        SplitDraft draft,
        CancellationToken cancellationToken = default)
    {
        if (draft == null)
            return ServiceResult<SplitBarcodeModel>.Fail("Kaydetme isteği (draft) boş olamaz.");

        if (string.IsNullOrWhiteSpace(draft.OriginalBarcode))
            return ServiceResult<SplitBarcodeModel>.Fail("Orijinal barkod boş olamaz.");

        if (draft.NewBarcodes == null || draft.NewBarcodes.Count == 0)
            return ServiceResult<SplitBarcodeModel>.Fail("Kaydedilecek yeni barkod bulunamadı.");

        try
        {
            var result = await _erp.SaveSplitAsync(draft, cancellationToken);

            if (!result.Success || result.Data == null)
                return ServiceResult<SplitBarcodeModel>.Fail(
                    result.Message ?? "Barkod bölme işlemi tamamlanamadı.");

            var model = SplitMapper.ToModel(result.Data);
            return ServiceResult<SplitBarcodeModel>.Ok(model, "Barkod bölme kaydedildi.");
        }
        catch (OperationCanceledException)
        {
            return ServiceResult<SplitBarcodeModel>.Fail("İşlem iptal edildi.", "CANCELED");
        }
        catch (Exception ex)
        {
            return ServiceResult<SplitBarcodeModel>.Fail(ex.Message, "UNHANDLED");
        }
    }
}

public interface ISplitService
{
    Task<ServiceResult<SplitBarcodeModel>> GetBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<SplitBarcodeModel>> SaveAsync(
        SplitDraft draft,
        CancellationToken cancellationToken = default);
}
