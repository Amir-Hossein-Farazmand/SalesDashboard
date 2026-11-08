using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Web.ViewModels;
namespace SalesDashboard.Web.Controllers;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class CustomersController(ICustomerService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? search = null)
    {
        var items = await service.GetAllAsync(search, HttpContext.RequestAborted);
        return View(new CustomersViewModel(search?.Trim(), items));
    }
}
