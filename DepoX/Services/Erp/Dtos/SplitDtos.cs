using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Services.Erp.Dtos;

public class SplitDraft
{
    public string OriginalBarcode { get; set; } = default!;
    public List<SplitNewBarcodeDraft> NewBarcodes { get; set; } = new();
}

public class SplitNewBarcodeDraft
{
    public string ItemCode { get; set; } = default!;
    public string ItemName { get; set; } = default!;
    public string LotCode { get; set; } = default!;
    public string ColorCode { get; set; } = default!;
    public string UnitCode { get; set; } = default!;
    public decimal Quantity { get; set; }
}

public class ErpBarcodeDetailDto
{
    public string Barcode { get; set; } = "";
    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";  
    public string LotCode { get; set; } = "";
    public string ColorCode { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public decimal Quantity { get; set; }

    public List<string> AvailableLots { get; set; } = new();
    public List<string> AvailableColors { get; set; } = new();
    public List<string> AvailableUnits { get; set; } = new();

    public List<ErpSplitBarcodeDto> ExistingSplits { get; set; } = new();
}

public class ErpSplitBarcodeDto
{
    public string Barcode { get; set; } = "";
    public string ItemCode { get; set; } = "";
    public string ItemName { get; set; } = "";
    public string LotCode { get; set; } = "";
    public string ColorCode { get; set; } = "";
    public string UnitCode { get; set; } = "";
    public decimal Quantity { get; set; }
}
