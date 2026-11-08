namespace SalesDashboard.Business.Models;

public sealed class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
}
