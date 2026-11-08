using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface ISalesService
{
    Task<IReadOnlyList<Sale>> GetAllAsync(int? sellerId, CancellationToken cancellationToken = default);
}
