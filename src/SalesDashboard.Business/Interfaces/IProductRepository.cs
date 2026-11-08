using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
}
