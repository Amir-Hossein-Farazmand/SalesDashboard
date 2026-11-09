// Represent the read-only Seller data returned by a WebAppDB view.
namespace SalesDashboard.Business.Models;

public sealed class Seller
{
    public int SellerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
}
