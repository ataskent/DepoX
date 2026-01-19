using DepoX.Services.Cache;
using DepoX.Services.Erp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Services
{
    public class AppStartupService
    {
        private readonly IErpGateway _erpGateway;
        private readonly IBarcodeCache _barcodeCache;

        public AppStartupService(
            IErpGateway erpGateway,
            IBarcodeCache barcodeCache)
        {
            _erpGateway = erpGateway;
            _barcodeCache = barcodeCache;
        }

        public async Task InitializeAsync()
        {
            //var barcodeMasters = await _erpGateway.GetBarcodeMastersAsync();
            //await _barcodeCache.LoadAsync(barcodeMasters);
        }
    }

}
