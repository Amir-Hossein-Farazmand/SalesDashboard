using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface IStockRepository
{
    Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken = default);
}
