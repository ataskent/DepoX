using DepoX.Dtos;
using DepoX.Features.Count;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DepoX.Services.Erp
{
    public class ErpGateway : IErpGateway
    {
        private readonly HttpClient _httpClient;

        public ErpGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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
                    return ErpResult<T>.Failed("ERP_EMPTY_RESPONSE", "ERP yanıtı boş.");

                var erpResponse = d.ToObject<ErpResponseDto<T>>();
                if (erpResponse == null)
                    return ErpResult<T>.Failed("ERP_PARSE_ERROR", "ERP yanıtı parse edilemedi.");

                if (!erpResponse.Success)
                    return ErpResult<T>.Failed(
                        string.IsNullOrWhiteSpace(erpResponse.ErrorCode) ? "ERP_BUSINESS_ERROR" : erpResponse.ErrorCode,
                        string.IsNullOrWhiteSpace(erpResponse.Message) ? "ERP işlemi başarısız." : erpResponse.Message);

                return ErpResult<T>.Ok(
                    erpResponse.Data,
                    string.IsNullOrWhiteSpace(erpResponse.Message) ? "İşlem başarılı." : erpResponse.Message,
                    erpResponse.ReferenceId);
            }
            catch (Exception ex)
            {
                return ErpResult<T>.Failed("ERP_JSON_ERROR", "ERP yanıtı okunamadı: " + ex.Message);
            }
        }

        public async Task<ErpResult<BasketDraftDto>> SaveBasketAsync(
            BasketDraftDto request,
            CancellationToken cancellationToken = default)
        {
            var url =
                "http://10.41.1.174:8061/customprg/xml/terminalservice.asmx/SaveBasket";

            return await PostAsync<BasketDraftDto>(
                url,
                new { draft = request },
                cancellationToken);
        }

        //public async Task<List<BarcodeMasterDto>> GetBarcodeMastersAsync()
        //{
        //    var url =
        //        "http://10.41.1.174:8061/customprg/xml/terminalservice.asmx/GetBarcodeMasters";

        //    var result = await PostAsync<List<BarcodeMasterDto>>(
        //        url,
        //        new { },          // payload yok / boş
        //        CancellationToken.None);

        //    if (!result.Success)
        //        throw new Exception(result.Message ?? "ERP barkod master hatası.");

        //    return result.Data ?? new List<BarcodeMasterDto>();
        //}

    }
}
