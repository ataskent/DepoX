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

        Task<ErpResult<NewBarcodeMetaDto>> GetNewBarcodeMetaAsync(
       CancellationToken cancellationToken = default);

        Task<ErpResult<List<string>>> GetLotsByItemAsync(
            string itemCode,
            CancellationToken cancellationToken = default);

        #endregion Split

        #region Basket
        
        Task<ErpResult<List<ErpWhouseDto>>> GetWhousesAsync(
            CancellationToken cancellationToken = default);

        Task<ErpResult<ErpBasketWhouseDto>> GetBasketDataAsync(
            CancellationToken cancellationToken = default);

        Task<ErpResult<List<BasketItemDto>>> GetBasketBarcodeDataAsync(
        string basketCode,
        CancellationToken cancellationToken = default);

        Task<ErpResult<ErpOptionalDto>> GetOptionalDataAsync(
            CancellationToken cancellationToken = default);

        #endregion Basket

        #region FromWorder
        Task<ErpResult<TransferList>> GetTransferAsync(
            CancellationToken cancellationToken = default);

        Task<ErpResult<TransferData>> GetTransferDataAsync(
            string transferCode,
            CancellationToken cancellationToken = default);

        Task<ErpResult<TransferData>> SaveTransferAsync(
            TransferData transferData,
            CancellationToken cancellationToken = default);

        #endregion FromWorder

        #region Count
        Task<ErpResult<CountM>> SaveCountAsync(
            CountM countM,
            CancellationToken cancellationToken = default);

        Task<ErpResult<CountList>> GetCountAsync(
        CancellationToken cancellationToken = default);

        Task<ErpResult<CountM>> GetCountDataAsync(
            string countCode,
            CancellationToken cancellationToken = default);

        #endregion

    }

}

