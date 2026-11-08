using Microsoft.EntityFrameworkCore;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Data.Repositories;

public sealed class ProductRepository(SalesDashboardDbContext context) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = context.Products.AsNoTracking();
        return await query.OrderBy(x => x.ProductName).ThenBy(x => x.ProductId).ToListAsync(cancellationToken);
    }
}
