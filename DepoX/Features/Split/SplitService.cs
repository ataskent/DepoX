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

    public async Task<SplitBarcodeModel> GetBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        var result = await _erp.GetBarcodeDetailAsync(barcode, cancellationToken);

        if (!result.Success || result.Data == null)
            throw new Exception(result.Message ?? "ERP barkod bilgisi alınamadı.");

        return SplitMapper.ToModel(result.Data);
    }

    public async Task<SplitBarcodeModel> SaveAsync(
        SplitDraft draft,
        CancellationToken cancellationToken = default)
    {        
        if (!draft.NewBarcodes.Any())
            return new SplitBarcodeModel();
        var result = await _erp.SaveSplitAsync(draft, cancellationToken);

        if (!result.Success || result.Data == null)
            throw new Exception(result.Message ?? "Barkod bölme işlemi tamamlanamadı.");

        return SplitMapper.ToModel(result.Data);
    }
}

public interface ISplitService
{
    Task<SplitBarcodeModel> GetBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default);

    Task<SplitBarcodeModel> SaveAsync(
        SplitDraft draft,
        CancellationToken cancellationToken = default);
}
