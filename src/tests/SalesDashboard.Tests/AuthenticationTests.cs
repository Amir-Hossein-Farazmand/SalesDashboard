// Verify UC-01 and UC-02 through HTTP, including cookies, validation and anti-forgery handling.
using System.Net;
using System.Text.RegularExpressions;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
namespace SalesDashboard.Tests;

public sealed class AuthenticationTests : IClassFixture<AppFactory>
{
    private readonly AppFactory _factory;
    public AuthenticationTests(AppFactory factory) => _factory = factory;
    private HttpClient Client() => _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    private static string Token(string html) => WebUtility.HtmlDecode(Regex.Match(html,
        "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value);
    private static async Task<HttpResponseMessage> Login(HttpClient client, string username = "admin", string password = "Demo@123", string? returnUrl = null)
    {
        var html = await client.GetStringAsync("/Auth/Login");
        var token = Token(html);
        Assert.NotEmpty(token);
        return await client.PostAsync("/Auth/Login", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Username"] = username,
            ["Password"] = password,
            ["ReturnURL"] = returnUrl ?? "",
            ["__RequestVerificationToken"] = token
        }));
    }

    [Fact(DisplayName = "TC-01 | UC-01 | ورود موفق و نمایش نام کامل")]
    public async Task TC01_ValidLoginShowsFullName()
    {
        using var client = Client();
        var response = await Login(client);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var home = await client.GetAsync("/Home/Index");
        Assert.Equal(HttpStatusCode.OK, home.StatusCode);
        var html = WebUtility.HtmlDecode(await home.Content.ReadAsStringAsync());
        Assert.Contains("System Administrator", html);
        Assert.DoesNotContain("Demo@123", html);
        Assert.True(home.Headers.CacheControl!.NoStore);
    }

    [Fact(DisplayName = "TC-02 | UC-01 | نام کاربری نامعتبر")]
    public async Task TC02_RejectedLogin()
    {
        using var client = Client();
        var response = await Login(client, "__missing_test_user__", "Demo@123");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        Assert.Contains("اطلاعات ورود معتبر نیست", html);
        var home = await client.GetAsync("/Home/Index");
        Assert.Equal(HttpStatusCode.Redirect, home.StatusCode);
        Assert.Contains("/Auth/Login", home.Headers.Location!.OriginalString);
    }

    [Fact(DisplayName = "TC-03 | UC-01 | رمز نادرست")]
    public async Task TC03_RejectedLogin()
    {
        using var client = Client();
        var response = await Login(client, "admin", "wrong");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        Assert.Contains("اطلاعات ورود معتبر نیست", html);
        var home = await client.GetAsync("/Home/Index");
        Assert.Equal(HttpStatusCode.Redirect, home.StatusCode);
        Assert.Contains("/Auth/Login", home.Headers.Location!.OriginalString);
    }

    [Fact(DisplayName = "TC-04 | UC-01 | حساب غیرفعال")]
    public async Task TC04_RejectedLogin()
    {
        using var client = Client();
        var response = await Login(client, "inactive", "Demo@123");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        Assert.Contains("اطلاعات ورود معتبر نیست", html);
        var home = await client.GetAsync("/Home/Index");
        Assert.Equal(HttpStatusCode.Redirect, home.StatusCode);
        Assert.Contains("/Auth/Login", home.Headers.Location!.OriginalString);
    }

    [Fact(DisplayName = "TC-05 | UC-02 | خروج و قطع دسترسی")]
    public async Task TC05_LogoutEndsAccess()
    {
        using var client = Client();
        var login = await Login(client);
        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);
        var home = await client.GetStringAsync("/Home/Index");
        var response = await client.PostAsync("/Auth/Logout", new FormUrlEncodedContent(new Dictionary<string, string>
        { ["__RequestVerificationToken"] = Token(home) }));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/Auth/Login", response.Headers.Location!.OriginalString);
        var protectedPage = await client.GetAsync("/Home/Index");
        Assert.Equal(HttpStatusCode.Redirect, protectedPage.StatusCode);
        Assert.Contains("/Auth/Login", protectedPage.Headers.Location!.OriginalString);
    }

    [Fact(DisplayName = "TC-16 | UC-01 | منع دسترسی بدون ورود")]
    public async Task TC16_AnonymousIsRedirectedToLogin()
    {
        using var client = Client();
        var response = await client.GetAsync("/Home/Index");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Auth/Login", response.Headers.Location!.OriginalString);
    }

    [Fact(DisplayName = "EX-01 | UC-01 | فرم خالی")]
    public async Task EX01_EmptyFormShowsValidation()
    {
        using var client = Client();
        var response = await Login(client, "", "");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
        Assert.Contains("نام کاربری را وارد کنید", html);
        Assert.Contains("رمز عبور را وارد کنید", html);
    }

    [Fact(DisplayName = "EX-02 | UC-02 | رد خروج بدون توکن")]
    public async Task EX02_LogoutWithoutTokenKeepsSession()
    {
        using var client = Client();
        var login = await Login(client);
        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);
        var response = await client.PostAsync("/Auth/Logout", new FormUrlEncodedContent(new Dictionary<string, string>()));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/Home/Index")).StatusCode);
    }
}
