using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Services.Erp.Dtos
{
    public class ErpBasketDraft
    {
        public Guid ClientDraftId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ErpBasketItem[] Items { get; set; }
    }

    public class ErpBasketItem
    {
        public string Barcode { get; set; }
        public decimal Quantity { get; set; }
    }

}
