// Read Sales data through the shared DbContext without tracking changes.
using Microsoft.EntityFrameworkCore;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Data.Repositories;

public sealed class SalesRepository(SalesDashboardDbContext context) : ISalesRepository
{
    public async Task<IReadOnlyList<Sale>> GetAllAsync(int? sellerId, CancellationToken cancellationToken = default)
    {
        var query = context.Sales.AsNoTracking();
        // Filter by stable seller ID and order by date and sale ID before materialization.
        if (sellerId.HasValue) query = query.Where(x => x.SellerId == sellerId.Value);
        return await query.OrderByDescending(x => x.SaleDate).ThenByDescending(x => x.SaleId).ToListAsync(cancellationToken);
    }
}
