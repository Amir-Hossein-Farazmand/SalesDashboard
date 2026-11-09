// Serve the authorized stock page through its business service.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesDashboard.Business.Interfaces;
namespace SalesDashboard.Web.Controllers;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class StockController(IStockService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index() =>
        View(await service.GetAllAsync(HttpContext.RequestAborted));
}
