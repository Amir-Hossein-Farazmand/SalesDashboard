using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(string? search, CancellationToken cancellationToken = default);
}
