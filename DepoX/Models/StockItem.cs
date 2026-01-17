using SQLite;

namespace DepoX.Models;

public class StockItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string ItemCode { get; set; }
    public string WhouseCode { get; set; }
    public int Quantity { get; set; }
}