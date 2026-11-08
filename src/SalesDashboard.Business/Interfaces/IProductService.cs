using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
}
