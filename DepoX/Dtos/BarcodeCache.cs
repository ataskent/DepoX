using DepoX.Dtos;

namespace DepoX.Services.Cache;

public class BarcodeCache : IBarcodeCache
{
    private readonly Dictionary<string, BarcodeMasterDto> _cache = new();

    public Task LoadAsync(IEnumerable<BarcodeMasterDto> barcodes)
    {
        _cache.Clear();

        foreach (var b in barcodes)
        {
            _cache[b.Barcode] = b;
        }

        return Task.CompletedTask;
    }

    public BarcodeMasterDto? GetByBarcode(string barcode)
        => _cache.TryGetValue(barcode, out var b) ? b : null;

    public bool Exists(string barcode)
        => _cache.ContainsKey(barcode);
}
