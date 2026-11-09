// Define the StockService contract without depending on EF Core or the web layer.
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface IStockService
{
    Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken = default);
}
