// Represent the read-only StockItem data returned by a WebAppDB view.
namespace SalesDashboard.Business.Models;

public sealed class StockItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
}
