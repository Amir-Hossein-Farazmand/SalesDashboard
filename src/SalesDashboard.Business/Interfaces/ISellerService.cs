using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface ISellerService
{
    Task<IReadOnlyList<Seller>> GetAllAsync(string? search, CancellationToken cancellationToken = default);
}
