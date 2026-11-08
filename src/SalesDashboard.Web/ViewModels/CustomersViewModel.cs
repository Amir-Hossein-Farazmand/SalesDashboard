using SalesDashboard.Business.Models;
namespace SalesDashboard.Web.ViewModels;

public sealed record CustomersViewModel(string? Search, IReadOnlyList<Customer> Items);
