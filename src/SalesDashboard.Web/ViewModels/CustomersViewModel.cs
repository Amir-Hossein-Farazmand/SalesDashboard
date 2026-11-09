// Keep the search text and customer results together for rendering.
using SalesDashboard.Business.Models;
namespace SalesDashboard.Web.ViewModels;

public sealed record CustomersViewModel(string? Search, IReadOnlyList<Customer> Items);
