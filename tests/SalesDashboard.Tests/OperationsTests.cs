using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
using SalesDashboard.Data;
using Xunit;
namespace SalesDashboard.Tests;

public sealed class OperationsFactory(string scenario = "normal") : WebApplicationFactory<Program>
{
    public OperationsData Data { get; } = new(scenario);
    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.ConfigureServices(services =>
    {
        services.RemoveAll<IUserRepository>();
        services.AddScoped<IUserRepository, TestUserRepository>();
        services.RemoveAll<ISellerRepository>();
        services.RemoveAll<IStockRepository>();
        services.RemoveAll<ISalesRepository>();
        services.AddSingleton<ISellerRepository>(Data);
        services.AddSingleton<IStockRepository>(Data);
        services.AddSingleton<ISalesRepository>(Data);
    });
}
public sealed class OperationsData(string scenario) : ISellerRepository, IStockRepository, ISalesRepository
{
    public int SalesCalls { get; private set; }
    public int? LastSellerId { get; private set; }
    public static IReadOnlyList<Sale> Rows => [
        new() { SaleId=1, SellerId=1, SellerName="Ali Rezaei", ProductName="UPS A", CustomerName="Customer A", Quantity=2, UnitPrice=1000, TotalAmount=2000, SaleDate=new DateOnly(2026,1,10) },
        new() { SaleId=2, SellerId=2, SellerName="Sara Mohammadi", ProductName="UPS B", CustomerName="Customer B", Quantity=3, UnitPrice=2000, TotalAmount=6000, SaleDate=new DateOnly(2026,2,11) }];
    private void Check() { if (scenario == "failure") throw new InvalidOperationException("Private SQL diagnostic"); }
    Task<IReadOnlyList<Seller>> ISellerRepository.GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        if (scenario == "sellerFailure") throw new InvalidOperationException("Private SQL diagnostic");
        return Task.FromResult<IReadOnlyList<Seller>>([
            new() { SellerId=1, FirstName="Ali", LastName="Rezaei" },
            new() { SellerId=2, FirstName="Sara", LastName="Mohammadi" },
            new() { SellerId=3, FirstName="No", LastName="Sales" }]);
    }
    Task<IReadOnlyList<StockItem>> IStockRepository.GetAllAsync(CancellationToken cancellationToken)
    {
        Check();
        return Task.FromResult<IReadOnlyList<StockItem>>(scenario == "empty" ? [] : [
            new() { ProductId=1, ProductName="Low Stock", Unit="Piece", StockQuantity=9 },
            new() { ProductId=2, ProductName="Boundary", Unit="Piece", StockQuantity=10 },
            new() { ProductId=3, ProductName="Zero Stock", Unit="Piece", StockQuantity=0 },
            new() { ProductId=4, ProductName="Enough", Unit="Piece", StockQuantity=11 }]);
    }
    Task<IReadOnlyList<Sale>> ISalesRepository.GetAllAsync(int? sellerId, CancellationToken cancellationToken)
    {
        Check(); SalesCalls++; LastSellerId = sellerId;
        return Task.FromResult<IReadOnlyList<Sale>>(scenario == "empty" ? [] : Rows.Where(x => !sellerId.HasValue || x.SellerId == sellerId.Value).ToList());
    }
}
public sealed class OperationsTests
{
    private static readonly string[] Routes = ["/Stock", "/Sales"];
    private static async Task<HttpClient> SignedIn(OperationsFactory factory)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var html = await client.GetStringAsync("/Auth/Login");
        var token = WebUtility.HtmlDecode(Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value);
        Assert.NotEmpty(token);
        using var response = await client.PostAsync("/Auth/Login", new FormUrlEncodedContent(new Dictionary<string, string>
        { ["Username"] = "admin", ["Password"] = "Demo@123", ["__RequestVerificationToken"] = token }));
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        return client;
    }
    private static async Task<string> Page(HttpClient client, string url)
    {
        using var response = await client.GetAsync(url); Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.CacheControl!.NoStore);
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }
    private static string Body(string html) => Regex.Match(html, @"<tbody>(.*?)</tbody>", RegexOptions.Singleline).Groups[1].Value;
    [Fact(DisplayName = "TC-11 | UC-06 | موجودی زیر ده قرمز")]
    public async Task TC11_LowStock()
    {
        using var f = new OperationsFactory(); using var c = await SignedIn(f);
        var html = await Page(c, "/Stock");
        Assert.Contains("class=\"stock-low\">9</td>", html);
        Assert.Contains("class=\"stock-low\">0</td>", html);
        Assert.Contains("Low Stock", html); Assert.Contains("Piece", html);
        var css = await c.GetStringAsync("/css/site.css");
        var rule = Regex.Match(
            css,
            @"\.data-table\s+td\.stock-low\s*\{([^}]*)\}",
            RegexOptions.IgnoreCase);

        Assert.Matches(
            @"(?i)(?:^|;)\s*color\s*:\s*#c62828\s*(?:;|$)",
            rule.Groups[1].Value);
    }
    [Fact(DisplayName = "TC-12 | UC-06 | موجودی ده قرمز نیست")]
    public async Task TC12_BoundaryStock()
    {
        using var f = new OperationsFactory(); using var c = await SignedIn(f); var html = await Page(c, "/Stock");
        Assert.Contains("class=\"stock-normal\">10</td>", html); Assert.Contains("class=\"stock-normal\">11</td>", html);
    }
    [Fact(DisplayName = "TC-13 | UC-07 | نمایش ستون‌های فروش و مبلغ View")]
    public async Task TC13_SalesList()
    {
        using var f = new OperationsFactory(); using var c = await SignedIn(f); var html = await Page(c, "/Sales");
        foreach (var value in new[] { "Ali Rezaei", "UPS A", "Customer A", "2,000", "2026-01-10", "Sara Mohammadi", "6,000", "مبلغ کل (ریال)" }) Assert.Contains(value, html);
        Assert.Equal(6, Regex.Matches(html, "<th scope=").Count); Assert.Null(f.Data.LastSellerId);
        // SQL multiplication is verified separately against SQL Server, not by canned data.
    }
    [Fact(DisplayName = "TC-14 | UC-07 | انتخاب فروشنده و پاک کردن فیلتر")]
    public async Task TC14_SellerFilter()
    {
        using var f = new OperationsFactory(); using var c = await SignedIn(f);
        var html = await Page(c, "/Sales?sellerId=2");
        Assert.Equal(2, f.Data.LastSellerId); Assert.Contains("UPS B", Body(html)); Assert.DoesNotContain("UPS A", Body(html));
        Assert.Matches("<option(?=[^>]*value=\"2\")(?=[^>]*selected=\"selected\")[^>]*>", html);
        html = await Page(c, "/Sales?sellerId="); Assert.Null(f.Data.LastSellerId); Assert.Contains("UPS A", Body(html)); Assert.Contains("UPS B", Body(html));
    }
    [Fact(DisplayName = "TC-15 | UC-07 | فروشنده معتبر بدون فروش")]
    public async Task TC15_NoSales()
    {
        using var f = new OperationsFactory(); using var c = await SignedIn(f);
        var html = await Page(c, "/Sales?sellerId=3"); Assert.Contains("موردی یافت نشد", html); Assert.Empty(Body(html));
    }
    [Fact(DisplayName = "TC-16-I3 | UC-06/07 | دسترسی ناشناس و پس از خروج")]
    public async Task TC16_AccessControl()
    {
        using var f = new OperationsFactory(); using var anon = f.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        foreach (var route in Routes) { using var r = await anon.GetAsync(route); Assert.Equal(HttpStatusCode.Redirect, r.StatusCode); Assert.Contains("/Auth/Login", r.Headers.Location!.OriginalString); }
        using var c = await SignedIn(f); var html = await Page(c, "/Stock");
        var token = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value;
        using var logout = await c.PostAsync("/Auth/Logout", new FormUrlEncodedContent(new Dictionary<string, string> { ["__RequestVerificationToken"] = token }));
        Assert.Equal(HttpStatusCode.Redirect, logout.StatusCode);
        foreach (var route in Routes) { using var r = await c.GetAsync(route); Assert.Equal(HttpStatusCode.Redirect, r.StatusCode); }
    }
    [Fact(DisplayName = "TC-17-I3 | UC-06/07 | نگاشت View و دقت مبلغ")]
    public void TC17_Mapping()
    {
        using var db = new SalesDashboardDbContext(new DbContextOptionsBuilder<SalesDashboardDbContext>().UseSqlServer("Server=localhost;Database=WebAppDB;Integrated Security=True;TrustServerCertificate=True").Options);
        foreach (var pair in new[] { (typeof(StockItem), "vw_Stock"), (typeof(Sale), "vw_Sales") })
        { var e = db.Model.FindEntityType(pair.Item1)!; Assert.Null(e.FindPrimaryKey()); Assert.Equal(pair.Item2, e.GetViewName()); Assert.Equal("dbo", e.GetViewSchema()); }
        Assert.Equal(29, db.Model.FindEntityType(typeof(Sale))!.FindProperty(nameof(Sale.TotalAmount))!.GetPrecision());
        Assert.Contains("[dbo].[vw_Stock]", db.Stock.ToQueryString());
        var query = db.Sales.Where(x => x.SellerId == 2).ToQueryString(); Assert.Contains("[dbo].[vw_Sales]", query); Assert.Contains("WHERE", query);
    }
    [Fact(DisplayName = "TC-18-I3 | UC-06/07 | صفحات فقط خواندنی")]
    public async Task TC18_ReadOnly()
    {
        using var f = new OperationsFactory(); using var c = await SignedIn(f);
        foreach (var route in Routes)
        {
            var html = await Page(c, route); foreach (var action in new[] { "/Create", "/Edit", "/Delete" }) Assert.DoesNotContain(action, html);
            using var response = await c.PostAsync(route + "/Index", new FormUrlEncodedContent(new Dictionary<string, string>())); Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
    }
    [Fact(DisplayName = "EX-06 | UC-06/07 | View خالی")]
    public async Task EX06_EmptyViews()
    {
        using var f = new OperationsFactory("empty"); using var c = await SignedIn(f);
        foreach (var route in Routes) { var html = await Page(c, route); Assert.Contains("موردی یافت نشد", html); Assert.Empty(Body(html)); }
    }
    [Fact(DisplayName = "EX-07 | UC-06/07 | خطای منبع داده")]
    public async Task EX07_DataFailure()
    {
        using var f = new OperationsFactory("failure"); using var c = await SignedIn(f);
        foreach (var route in Routes)
        { using var r = await c.GetAsync(route); Assert.Equal(HttpStatusCode.ServiceUnavailable, r.StatusCode); var html = await r.Content.ReadAsStringAsync(); Assert.DoesNotContain("Private SQL diagnostic", html); Assert.Empty(Body(html)); }
        using var sf = new OperationsFactory("sellerFailure"); using var sc = await SignedIn(sf); using var sr = await sc.GetAsync("/Sales");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, sr.StatusCode); Assert.Equal(0, sf.Data.SalesCalls);
    }
    [Fact(DisplayName = "EX-08 | UC-07 | فیلتر دستکاری‌شده")]
    public async Task EX08_InvalidFilter()
    {
        using var f = new OperationsFactory(); using var c = await SignedIn(f);
        foreach (var value in new[] { "abc", "-1", "0", "999", "2147483648" })
        { using var r = await c.GetAsync("/Sales?sellerId=" + value); Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode); }
        Assert.Equal(0, f.Data.SalesCalls);
    }
}
