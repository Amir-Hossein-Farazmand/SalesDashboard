using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Services;

public sealed class SalesService(ISalesRepository repository) : ISalesService
{
    public Task<IReadOnlyList<Sale>> GetAllAsync(int? sellerId, CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(sellerId, cancellationToken);
}
