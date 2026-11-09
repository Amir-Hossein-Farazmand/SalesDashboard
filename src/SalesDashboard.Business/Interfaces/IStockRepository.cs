// Define the StockRepository contract without depending on EF Core or the web layer.
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface IStockRepository
{
    Task<IReadOnlyList<StockItem>> GetAllAsync(CancellationToken cancellationToken = default);
}
