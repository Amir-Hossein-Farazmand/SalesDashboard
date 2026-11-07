using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace SalesDashboard.Web.Controllers;

[Authorize]
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class HomeController : Controller
{
    // مقصد موقت دور اول مطابق طراحی VPP؛ صفحه کسب‌وکاری تازه نیست.
    public IActionResult Index() => View();
}