namespace DepoX.Features.Count;

public class CountMVm
{
    public string DocNo { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string WhouseCode { get; set; } = string.Empty;
    public string WhouseName { get; set; } = string.Empty;
    public List<ItemVm> Items { get; set; } = new();
    public List<BarcodeVm> Barcodes { get; set; } = new();
}

public class CountListVm
{
    public List<CountVm> Counts { get; set; } = new();
    public List<WhouseVm> Whouses { get; set; } = new();
}

public class CountVm
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class WhouseVm
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class ItemVm
{
    public decimal Quantity { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class BarcodeVm
{
    public string Code { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public bool IsEditing { get; set; }
    public bool IsSelected { get; set; }
}
