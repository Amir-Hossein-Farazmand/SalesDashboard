// Read Customer data through the shared DbContext without tracking changes.
using Microsoft.EntityFrameworkCore;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Data.Repositories;

public sealed class CustomerRepository(SalesDashboardDbContext context) : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = context.Customers.AsNoTracking();
        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.FullName.Contains(search));
        return await query.OrderBy(x => x.FullName).ThenBy(x => x.CustomerId).ToListAsync(cancellationToken);
    }
}
