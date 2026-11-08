using SalesDashboard.Business.Models;
namespace SalesDashboard.Web.ViewModels;

public sealed record SellersViewModel(string? Search, IReadOnlyList<Seller> Items);
