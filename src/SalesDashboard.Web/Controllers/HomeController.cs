// Render the existing authenticated landing page without querying business data.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace SalesDashboard.Web.Controllers;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class HomeController : Controller
{
    // Keep the original landing page; it is not an additional business use case.
    public IActionResult Index() => View();
}