using DepoX.Dtos;
using DepoX.Services.Erp.Dtos;

namespace DepoX.Services.Erp
{
    public interface IErpGateway
    {
        Task<ErpResult<ErpBasketDraft>> SaveBasketAsync(
            ErpBasketDraft draft,
            CancellationToken cancellationToken = default);

        #region Split
        Task<ErpResult<ErpBarcodeDetailDto>> GetBarcodeDetailAsync(
            string barcode,
            CancellationToken cancellationToken = default);

        Task<ErpResult<ErpBarcodeDetailDto>> SaveSplitAsync(
            SplitDraft request,
            CancellationToken cancellationToken = default); 

        Task<ErpResult<ErpBarcodeDetailDto>> CreateBarcodeAsync(
            SplitNewBarcodeDraft request,
            CancellationToken cancellationToken = default);

        #endregion Split

    }

}

