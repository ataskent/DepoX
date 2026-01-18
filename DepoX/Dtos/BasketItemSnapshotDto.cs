using System;

namespace DepoX.Dtos
{
    public class BasketItemSnapshotDto
    {
        public string Barcode { get; set; }
        public string StockCode { get; set; }
        public decimal Quantity { get; set; }
        public string FromWarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public string AddedByTerminalId { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
