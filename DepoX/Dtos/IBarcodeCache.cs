using DepoX.Dtos;

namespace DepoX.Services.Cache;

public interface IBarcodeCache
{
    Task LoadAsync(IEnumerable<BarcodeMasterDto> barcodes);
    BarcodeMasterDto? GetByBarcode(string barcode);
    bool Exists(string barcode);
}
