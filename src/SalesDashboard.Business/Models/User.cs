namespace SalesDashboard.Business.Models;

public sealed class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}