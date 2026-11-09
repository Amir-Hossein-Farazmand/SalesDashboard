// Represent the read-only Sale data returned by a WebAppDB view.
namespace SalesDashboard.Business.Models;

public sealed class Sale
{
    public int SaleId { get; set; }
    public int SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public DateOnly SaleDate { get; set; }
}
