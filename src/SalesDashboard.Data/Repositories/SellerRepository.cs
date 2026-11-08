using Microsoft.EntityFrameworkCore;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Data.Repositories;

public sealed class SellerRepository(SalesDashboardDbContext context) : ISellerRepository
{
    public async Task<IReadOnlyList<Seller>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = context.Sellers.AsNoTracking();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => (x.FirstName + " " + x.LastName).Contains(search));
        return await query.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).ThenBy(x => x.SellerId).ToListAsync(cancellationToken);
    }
}
