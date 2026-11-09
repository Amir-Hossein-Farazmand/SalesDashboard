// Validate the selected seller, load all seller choices, and display the matching read-only sales.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Web.ViewModels;
namespace SalesDashboard.Web.Controllers;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class SalesController(ISalesService salesService, ISellerService sellerService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? sellerId = null)
    {
        if (!ModelState.IsValid) return BadRequest("فیلتر فروشنده معتبر نیست.");
        var ct = HttpContext.RequestAborted;
        // Use the seller view so sellers without sales remain selectable.
        var sellers = await sellerService.GetAllAsync(null, ct);
        // An unknown ID is an invalid filter, not a request for all sales.
        if (sellerId.HasValue && !sellers.Any(x => x.SellerId == sellerId.Value))
            return BadRequest("فیلتر فروشنده معتبر نیست.");
        var items = await salesService.GetAllAsync(sellerId, ct);
        return View(new SalesViewModel(sellerId, sellers, items));
    }
}
