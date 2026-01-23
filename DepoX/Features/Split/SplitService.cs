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

    // ===============================
    // GET BARCODE
    // ===============================

    public async Task<ServiceResult<SplitBarcodeModel>> GetBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return ServiceResult<SplitBarcodeModel>.Fail("Barkod boş olamaz.");

        var result = await _erp.GetBarcodeDetailAsync(barcode.Trim(), cancellationToken);

        if (!result.Success || result.Data == null)
            return ServiceResult<SplitBarcodeModel>.Fail(
                result.Message ?? "ERP barkod bilgisi alınamadı.");

        return ServiceResult<SplitBarcodeModel>.Ok(
            SplitMapper.ToModel(result.Data));
    }

    // ===============================
    // SAVE SPLIT
    // ===============================

    public async Task<ServiceResult<SplitBarcodeModel>> SaveAsync(
        SplitDraft draft,
        CancellationToken cancellationToken = default)
    {
        if (draft == null)
            return ServiceResult<SplitBarcodeModel>.Fail("Split taslağı boş.");

        if (string.IsNullOrWhiteSpace(draft.OriginalBarcode))
            return ServiceResult<SplitBarcodeModel>.Fail("Orijinal barkod boş.");

        if (draft.NewBarcodes == null || !draft.NewBarcodes.Any())
            return ServiceResult<SplitBarcodeModel>.Fail("Yeni barkod yok.");

        var result = await _erp.SaveSplitAsync(draft, cancellationToken);

        if (!result.Success || result.Data == null)
            return ServiceResult<SplitBarcodeModel>.Fail(
                result.Message ?? "Barkod bölme kaydedilemedi.");

        return ServiceResult<SplitBarcodeModel>.Ok(
            SplitMapper.ToModel(result.Data),
            "Barkod bölme başarıyla kaydedildi.");
    }

    // ===============================
    // CREATE NEW BARCODE
    // ===============================

    public async Task<ServiceResult<SplitBarcodeModel>> CreateNewBarcodeAsync(
        SplitNewBarcodeDraft draft,
        CancellationToken cancellationToken = default)
    {
        if (draft == null)
            return ServiceResult<SplitBarcodeModel>.Fail("Yeni barkod taslağı boş.");

        if (string.IsNullOrWhiteSpace(draft.ItemCode))
            return ServiceResult<SplitBarcodeModel>.Fail("Stok kodu boş.");

        if (draft.Quantity <= 0)
            return ServiceResult<SplitBarcodeModel>.Fail("Miktar 0 veya negatif olamaz.");

        if (string.IsNullOrWhiteSpace(draft.UnitCode))
            return ServiceResult<SplitBarcodeModel>.Fail("Birim kodu boş.");

        var result = await _erp.CreateBarcodeAsync(draft, cancellationToken);

        if (!result.Success || result.Data == null)
            return ServiceResult<SplitBarcodeModel>.Fail(
                result.Message ?? "Yeni barkod oluşturulamadı.");

        return ServiceResult<SplitBarcodeModel>.Ok(
            SplitMapper.ToModel(result.Data),
            "Yeni barkod başarıyla oluşturuldu.");
    }

    public async Task<ServiceResult<NewBarcodeMetaDto>> GetNewBarcodeMetaAsync()
    {
        var result = await _erp.GetNewBarcodeMetaAsync();
        if (!result.Success || result.Data == null)
            return ServiceResult<NewBarcodeMetaDto>.Fail(result.Message);

        return ServiceResult<NewBarcodeMetaDto>.Ok(result.Data);
    }

    public async Task<ServiceResult<List<string>>> GetLotsByItemAsync(string itemCode)
    {
        var result = await _erp.GetLotsByItemAsync(itemCode);
        if (!result.Success || result.Data == null)
            return ServiceResult<List<string>>.Fail(result.Message);

        return ServiceResult<List<string>>.Ok(result.Data);
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

    Task<ServiceResult<SplitBarcodeModel>> CreateNewBarcodeAsync(
        SplitNewBarcodeDraft draft,
        CancellationToken cancellationToken = default);


    // 🔽 YENİ EKLENENLER
    Task<ServiceResult<NewBarcodeMetaDto>> GetNewBarcodeMetaAsync();

    Task<ServiceResult<List<string>>> GetLotsByItemAsync(string itemCode);
}
