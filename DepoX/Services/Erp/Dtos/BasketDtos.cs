namespace DepoX.Services.Erp.Dtos;

public class ErpBasketDraft
{
    public string BasketNo { get; set; } = string.Empty;
    public string WhouseCode { get; set; } = string.Empty;
    public Guid ClientDraftId { get; set; }
    public DateTime CreatedAt { get; set; }
    public ErpBasketItem[] Items { get; set; } = Array.Empty<ErpBasketItem>();
}

public class ErpBasketItem
{
    public string Barcode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class ErpWhouseDto
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public string BranchCode { get; set; } = "";
}

public class ErpBasketDto
{
    public string Code { get; set; } = string.Empty;

}

public class ErpBranchDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

}

public class ErpOptionalDto
{
    public List<ErpBranchDto> branches { get; set; } = new();
    public List<ErpWhouseDto> whouses { get; set; } = new();
}

public class ErpBasketWhouseDto
{
    public List<ErpBasketDto> baskets { get; set; } = new();
    public List<ErpWhouseDto> whouses { get; set; } = new();
}

public class BasketItemDto
{
    public string Barcode { get; set; } = string.Empty;
    public string StockCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 0;
    public string FromWarehouseCode { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
}
