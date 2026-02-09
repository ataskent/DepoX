using DepoX.Dtos;
using DepoX.Services.Erp.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace DepoX.Services.Erp
{
    public class ErpGateway : IErpGateway
    {
        private readonly HttpClient _httpClient; 
        private const string BaseUrl = "http://10.41.1.174:8061/customprg/xml/terminalservice.asmx";

        public ErpGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region IErpGateway IMPLEMENTASYONU

        // 🔹 Generic ERP POST helper (SADECE Gateway içi)
        private async Task<ErpResult<T>> PostAsync<T>(
            string url,
            object payload,
            CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(payload);

            using var content =
                new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync(url, content, cancellationToken);
            }
            catch (Exception ex)
            {
                return ErpResult<T>.Failed("ERP_CONNECT_ERROR", ex.Message);
            }

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return ErpResult<T>.Failed("ERP_HTTP_ERROR", responseText);

            try
            {
                var root = JObject.Parse(responseText);
                var d = root["d"];

                if (d == null)
                    return ErpResult<T>.Failed(
                        "ERP_EMPTY_RESPONSE",
                        "ERP yanıtı boş.");

                var erpResponse = d.ToObject<ErpResponseDto<T>>();
                if (erpResponse == null)
                    return ErpResult<T>.Failed(
                        "ERP_PARSE_ERROR",
                        "ERP yanıtı parse edilemedi.");

                if (!erpResponse.Success)
                    return ErpResult<T>.Failed(
                        string.IsNullOrWhiteSpace(erpResponse.ErrorCode)
                            ? "ERP_BUSINESS_ERROR"
                            : erpResponse.ErrorCode,
                        string.IsNullOrWhiteSpace(erpResponse.Message)
                            ? "ERP işlemi başarısız."
                            : erpResponse.Message);

                return ErpResult<T>.Ok(
                    erpResponse.Data,
                    string.IsNullOrWhiteSpace(erpResponse.Message)
                        ? "İşlem başarılı."
                        : erpResponse.Message,
                    erpResponse.ReferenceId);
            }
            catch (Exception ex)
            {
                return ErpResult<T>.Failed(
                    "ERP_JSON_ERROR",
                    "ERP yanıtı okunamadı: " + ex.Message);
            }
        }

        #endregion

        #region Split

        public async Task<ErpResult<ErpBarcodeDetailDto>> GetBarcodeDetailAsync(
            string barcode,
            CancellationToken cancellationToken = default)
        {
            var url =
                $"{BaseUrl}/GetBarcodeDetail";

            return await PostAsync<ErpBarcodeDetailDto>(
                url,
                new { barcode },
                cancellationToken);
        }

        public async Task<ErpResult<ErpBarcodeDetailDto>> SaveSplitAsync(
           SplitDraft request,
           CancellationToken cancellationToken = default)
        {
            var url =
                $"{BaseUrl}/SaveBarcodeSplit";

            return await PostAsync<ErpBarcodeDetailDto>(
                url,
                new { draft = request },
                cancellationToken);
        }
        public async Task<ErpResult<ErpBarcodeDetailDto>> CreateBarcodeAsync(
            SplitNewBarcodeDraft request,
            CancellationToken cancellationToken = default)
        {
            var url =
                $"{BaseUrl}/CreateBarcode";

            return await PostAsync<ErpBarcodeDetailDto>(
                url,
                new { draft = request },
                cancellationToken);
        }

        // ===============================
        // YENİ BARKOD META
        // ===============================

        public Task<ErpResult<NewBarcodeMetaDto>> GetNewBarcodeMetaAsync(
            CancellationToken cancellationToken = default)
            => PostAsync<NewBarcodeMetaDto>(
                $"{BaseUrl}/GetNewBarcodeMeta",
                new { }, cancellationToken);

        // ===============================
        // LOT BY ITEM
        // ===============================

        public Task<ErpResult<List<string>>> GetLotsByItemAsync(
            string itemCode,
            CancellationToken cancellationToken = default)
            => PostAsync<List<string>>(
                $"{BaseUrl}/GetLotsByItem",
                new { itemCode }, cancellationToken);

        #endregion Split

        #region Basket

        // 🔹 IErpGateway IMPLEMENTASYONU
        public Task<ErpResult<ErpBasketDraft>> SaveBasketAsync(
            ErpBasketDraft request,
            CancellationToken cancellationToken = default)
        {
            const string url =
                $"{BaseUrl}/SaveBasket";

            return PostAsync<ErpBasketDraft>(
                url,
                new { draft = request },
                cancellationToken);
        }

        public Task<ErpResult<List<ErpWhouseDto>>> GetWhousesAsync(
    CancellationToken cancellationToken = default)
        {
            return PostAsync<List<ErpWhouseDto>>(
                $"{BaseUrl}/GetWhouses",
                new { },
                cancellationToken);
        }

        public Task<ErpResult<ErpBasketWhouseDto>> GetBasketDataAsync(
CancellationToken cancellationToken = default)
        {
            return PostAsync<ErpBasketWhouseDto>(
                $"{BaseUrl}/GetBasketWhouseData",
                new { },
                cancellationToken);
        }

        public Task<ErpResult<List<BasketItemDto>>> GetBasketBarcodeDataAsync(
     string basketCode,
     CancellationToken cancellationToken = default)
        {
            return PostAsync<List<BasketItemDto>>(
                $"{BaseUrl}/GetBasketBarcodeData",
                new { BasketBarcode = basketCode },
                cancellationToken);
        }

        public Task<ErpResult<bool>> ClearBasketDataAsync(
     string basketCode, string whouseCode, 
     CancellationToken cancellationToken = default)
        {
            return PostAsync<bool>(
                $"{BaseUrl}/CloseBasketData",
                new { BasketBarcode = basketCode , WhouseCode = whouseCode },                
                cancellationToken);
        }

        public Task<ErpResult<bool>> DeleteBasketDataAsync(
   string basketCode,
   CancellationToken cancellationToken = default)
        {
            return PostAsync<bool>(
                $"{BaseUrl}/DeleteBasketData",
                new { BasketBarcode = basketCode },
                cancellationToken);
        }


        public Task<ErpResult<ErpOptionalDto>> GetOptionalDataAsync(
CancellationToken cancellationToken = default)
        {
            return PostAsync<ErpOptionalDto>(
                $"{BaseUrl}/GetOptionalData",
                new { },
                cancellationToken);
        }

        public Task<ErpResult<TransferList>> GetTransferAsync(
CancellationToken cancellationToken = default)
        {
            return PostAsync<TransferList>(
                $"{BaseUrl}/GetTransfer",
                new { },
                cancellationToken);
        }

        public Task<ErpResult<TransferData>> GetTransferDataAsync(
            string transferCode,
CancellationToken cancellationToken = default)
        {
            return PostAsync<TransferData>(
                $"{BaseUrl}/GetTransferData",
                new { TransferCode = transferCode },
                cancellationToken);
        }

        public Task<ErpResult<TransferData>> SaveTransferAsync(
            TransferData transferData,
CancellationToken cancellationToken = default)
        {
            return PostAsync<TransferData>(
                $"{BaseUrl}/SaveTransferData",
                new { dto = transferData },
                cancellationToken);

        }


        public Task<ErpResult<CountM>> SaveCountAsync(
            CountM countM,
            CancellationToken cancellationToken = default)
        {
            return PostAsync<CountM>(
                $"{BaseUrl}/SaveCountData",
                new { dto = countM },
                cancellationToken);

        }

        public Task<ErpResult<CountList>> GetCountAsync(
            CancellationToken cancellationToken = default)
        {
            return PostAsync<CountList>(
                $"{BaseUrl}/GetCount",
                new { },
                cancellationToken);
        }

        public Task<ErpResult<CountM>> GetCountDataAsync(
            string countCode,
            CancellationToken cancellationToken = default)
        {
            return PostAsync<CountM>(
                $"{BaseUrl}/GetCountData",
                new { CountCode = countCode },
                cancellationToken);
        }


        #endregion Basket

        #region General
        public async Task<bool> IsConnectAsync(CancellationToken cancellationToken)
        {
            try
            {
                var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    $"{BaseUrl}/IsConnect")
                {
                    Content = new FormUrlEncodedContent(
                        Array.Empty<KeyValuePair<string, string>>())
                };

                var response = await _httpClient.SendAsync(
                    request,
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return false;

                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                // ASMX dönüşleri:
                // <boolean>true</boolean>
                // veya <string>true</string>
                // veya JSON string içinde true
                return content.Contains("true", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
        #endregion

    }
}
