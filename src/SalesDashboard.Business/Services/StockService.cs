// Coordinate stock reads through the repository contract.
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Services;

public sealed class StockService(IStockRepository repository) : IStockService
{
    public Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(cancellationToken);
}
