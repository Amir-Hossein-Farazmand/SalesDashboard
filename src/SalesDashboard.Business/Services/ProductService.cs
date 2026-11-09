// Coordinate product reads through the repository contract.
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);
}
