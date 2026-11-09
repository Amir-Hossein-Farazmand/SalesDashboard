// Define the SellerService contract without depending on EF Core or the web layer.
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface ISellerService
{
    Task<IReadOnlyList<Seller>> GetAllAsync(string? search, CancellationToken cancellationToken = default);
}
