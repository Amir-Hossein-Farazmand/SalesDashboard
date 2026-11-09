// Handle login and logout with cookie authentication; business credential checks stay in IAuthService.
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Web.ViewModels;
namespace SalesDashboard.Web.Controllers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class AuthController(IAuthService authService) : Controller
{
    private readonly IAuthService _authService = authService;
    [AllowAnonymous, HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
        return View(new LoginViewModel { ReturnURL = returnUrl });
    }
    [AllowAnonymous, HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return Redisplay(model);
        var user = await _authService.LoginAsync(model.Username, model.Password, HttpContext.RequestAborted);
        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "اطلاعات ورود معتبر نیست یا حساب غیرفعال است.");
            return Redisplay(model);
        }
        // Store the user ID and full name in the cookie identity, never the password.
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString(CultureInfo.InvariantCulture)),
            new Claim(ClaimTypes.Name, user.FullName)
        };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
            new AuthenticationProperties { IsPersistent = false });
        // Only allow local return URLs after a successful sign-in.
        if (!string.IsNullOrEmpty(model.ReturnURL) && Url.IsLocalUrl(model.ReturnURL))
            return LocalRedirect(model.ReturnURL);
        return RedirectToAction("Index", "Home");
    }
    [Authorize, HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
    private ViewResult Redisplay(LoginViewModel model)
    {
        // Clear both the model and binding state before redisplaying the form.
        model.Password = string.Empty;
        ModelState.SetModelValue(nameof(model.Password), new Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderResult(string.Empty));
        return View("Login", model);
    }
}