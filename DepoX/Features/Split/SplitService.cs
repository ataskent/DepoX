using DepoX.Services.Erp;
using DepoX.Services.Erp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Features.Split;

public class SplitService : ISplitService
{
    private readonly IErpGateway _erp;

    public SplitService(IErpGateway erp)
    {
        _erp = erp;
    }

    public Task SaveAsync(SplitDraft draft, CancellationToken cancellationToken = default)
    {
        // ERP çağrısı buraya gelecek
        return Task.CompletedTask;
    }
    

    public async Task<ErpBarcodeDetailDto> GetBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        var result = await _erp.GetBarcodeDetailAsync(barcode, cancellationToken);

        if (!result.Success || result.Data == null)
            throw new Exception(result.Message ?? "ERP barkod bilgisi alınamadı.");

        return result.Data;
    }
}

public interface ISplitService
{
    Task SaveAsync(SplitDraft draft, CancellationToken cancellationToken = default);

    Task<ErpBarcodeDetailDto> GetBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default);


}
