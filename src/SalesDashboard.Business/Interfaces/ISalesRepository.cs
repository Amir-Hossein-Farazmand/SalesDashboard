// Define the SalesRepository contract without depending on EF Core or the web layer.
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface ISalesRepository
{
    Task<IReadOnlyList<Sale>> GetAllAsync(int? sellerId, CancellationToken cancellationToken = default);
}
