using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Services
{
    public class BarcodeScannerService
    {
        public event Action<string> OnBarcodeScanned;

        public void Scan(string barcode)
        {
            OnBarcodeScanned?.Invoke(barcode);
        }

    }
}
