using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Services;

public sealed class SellerService(ISellerRepository repository) : ISellerService
{
    public Task<IReadOnlyList<Seller>> GetAllAsync(string? search, CancellationToken cancellationToken = default) =>
        repository.GetAllAsync(string.IsNullOrWhiteSpace(search) ? null : search.Trim(), cancellationToken);
}
