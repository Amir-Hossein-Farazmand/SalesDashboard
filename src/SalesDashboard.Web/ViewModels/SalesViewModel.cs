using SalesDashboard.Business.Models;
namespace SalesDashboard.Web.ViewModels;

public sealed record SalesViewModel(int? SellerId, IReadOnlyList<Seller> Sellers, IReadOnlyList<Sale> Items);
