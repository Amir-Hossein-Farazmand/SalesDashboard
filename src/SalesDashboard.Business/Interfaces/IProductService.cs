// Define the ProductService contract without depending on EF Core or the web layer.
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
}
