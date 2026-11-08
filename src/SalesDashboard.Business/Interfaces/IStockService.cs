using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface IStockService
{
    Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken = default);
}
