using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Web.ViewModels;
namespace SalesDashboard.Web.Controllers;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class ProductsController(IProductService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await service.GetAllAsync(HttpContext.RequestAborted);
        return View(items);
    }
}
