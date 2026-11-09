// Define the CustomerRepository contract without depending on EF Core or the web layer.
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(string? search, CancellationToken cancellationToken = default);
}
