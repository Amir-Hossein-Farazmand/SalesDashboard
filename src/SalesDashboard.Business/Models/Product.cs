// Represent the read-only Product data returned by a WebAppDB view.
namespace SalesDashboard.Business.Models;

public sealed class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal? LatestPrice { get; set; }
    public DateOnly? LatestPriceStartDate { get; set; }
}
