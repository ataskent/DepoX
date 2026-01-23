using DepoX.Services.Erp.Dtos;

namespace DepoX.Features.Split;

public static class SplitMapper
{
    // ===============================
    // ERP → DOMAIN
    // ===============================

    public static SplitBarcodeModel ToModel(ErpBarcodeDetailDto dto)
        => new()
        {
            Barcode = dto.Barcode,
            ItemCode = dto.ItemCode,
            ItemName = dto.ItemName,
            LotCode = dto.LotCode,
            ColorCode = dto.ColorCode,
            UnitCode = dto.UnitCode,
            Quantity = dto.Quantity,

            AvailableLots = dto.AvailableLots,
            AvailableColors = dto.AvailableColors,
            AvailableUnits = dto.AvailableUnits,

            ExistingSplits = dto.ExistingSplits
                .Select(ToModel)
                .ToList()
        };

    private static SplitBarcodeModel ToModel(ErpSplitBarcodeDto dto)
        => new()
        {
            Barcode = dto.Barcode,
            ItemCode = dto.ItemCode,
            ItemName = dto.ItemName,
            LotCode = dto.LotCode,
            ColorCode = dto.ColorCode,
            UnitCode = dto.UnitCode,
            Quantity = dto.Quantity
        };

    // ===============================
    // DOMAIN → VM
    // ===============================

    public static SplitRowVm ToVm(SplitBarcodeModel model, bool isExisting)
        => new()
        {
            IsExisting = isExisting,
            Barcode = model.Barcode,
            ItemCode = model.ItemCode,
            ItemName = model.ItemName,
            LotCode = model.LotCode,
            ColorCode = model.ColorCode,
            UnitCode = model.UnitCode,
            Quantity = model.Quantity
        };

    // ===============================
    // VM → ERP (SPLIT)
    // ===============================

    public static SplitDraft ToErpDraft(
        string originalBarcode,
        IEnumerable<SplitRowVm> newSplits)
        => new()
        {
            OriginalBarcode = originalBarcode,
            NewBarcodes = newSplits
                .Select(x => new SplitNewBarcodeDraft
                {
                    ItemCode = x.ItemCode,
                    LotCode = x.LotCode,
                    ColorCode = x.ColorCode,
                    UnitCode = x.UnitCode,
                    Quantity = x.Quantity
                })
                .ToList()
        };

    // ===============================
    // VM → ERP (NEW BARCODE)
    // ===============================
    public static string ExtractCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "";

        var idx = value.IndexOf('-');
        return idx > 0
            ? value.Substring(0, idx).Trim()
            : value.Trim();
    }

    public static SplitNewBarcodeDraft ToNewBarcodeDraft(SplitRowVm vm)
        => new()
        {
            ItemCode = vm.ItemCode,
            LotCode = vm.LotCode,
            ColorCode = ExtractCode(vm.ColorCode),
            UnitCode = vm.UnitCode,
            Quantity = vm.Quantity,
            NewBarcode = vm.NewBarcode
        };
}