using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Services.Erp.Dtos;

public class CountM
{
    public string DocNo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string WhouseCode { get; set; } = string.Empty;
    public string WhouseName { get; set; } = string.Empty;
    public List<ItemCount> Items { get; set; } = new();
    public List<BarcodeCount> Barcodes { get; set; } = new();
}

public class CountList
{
    public List<Count> Counts { get; set; } = new();
    public List<WhouseCount> Whouses { get; set; } = new();
}

public class Count
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class WhouseCount
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}


public class ItemCount
{
    public decimal Quantity { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class BarcodeCount
{
    public string Code { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public bool IsEditing { get; set; }
    public bool IsSelected { get; set; }
}

