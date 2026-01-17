using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;

namespace DepoX.Models
{
    public class StockTransaction
    {
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ProductCode { get; set; }
        public string FromWhouse { get; set; }
        public string ToWhouse { get; set; }
        public int Quantity { get; set; }
        public string OperationType { get; set; } // "TRANSFER", "COUNT", "SHIPMENT"
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool Synced { get; set; } = false;
    }
}
