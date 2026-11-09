// Keep the search text and seller results together for rendering.
using SalesDashboard.Business.Models;
namespace SalesDashboard.Web.ViewModels;

public sealed record SellersViewModel(string? Search, IReadOnlyList<Seller> Items);
