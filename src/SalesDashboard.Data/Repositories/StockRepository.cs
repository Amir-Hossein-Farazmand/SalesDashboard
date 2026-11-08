using Microsoft.EntityFrameworkCore;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Data.Repositories;

public sealed class StockRepository(SalesDashboardDbContext context) : IStockRepository
{
    public async Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = context.Stock.AsNoTracking();
        return await query.OrderBy(x => x.ProductName).ThenBy(x => x.ProductId).ToListAsync(cancellationToken);
    }
}
