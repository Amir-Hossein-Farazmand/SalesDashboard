using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Services;

public sealed class CustomerService(ICustomerRepository repository) : ICustomerService
{
	public Task<IReadOnlyList<Customer>> GetAllAsync(string? search, CancellationToken cancellationToken = default) =>
		repository.GetAllAsync(string.IsNullOrWhiteSpace(search) ? null : search.Trim(), cancellationToken);
}
