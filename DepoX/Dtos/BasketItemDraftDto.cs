namespace DepoX.Dtos
{
    public class BasketItemDraftDto
    {
        public string Barcode { get; set; }
        public string StockCode { get; set; }
        public decimal Quantity { get; set; }
        public string FromWarehouseCode { get; set; }
        public string LocationCode { get; set; }
    }
}
