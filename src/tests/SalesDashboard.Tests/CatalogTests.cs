// Verify UC-03 through UC-05 using controlled repositories; metadata checks do not execute SQL Server.
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

public sealed class CatalogFactory(string scenario = "normal") : WebApplicationFactory<Program>
{
    public CatalogData Data { get; } = new(scenario);
    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.ConfigureServices(services =>
    {
        services.RemoveAll<IUserRepository>();
        services.AddScoped<IUserRepository, TestUserRepository>();
        services.RemoveAll<ISellerRepository>();
        services.RemoveAll<ICustomerRepository>();
        services.RemoveAll<IProductRepository>();
        services.AddSingleton<ISellerRepository>(Data);
        services.AddSingleton<ICustomerRepository>(Data);
        services.AddSingleton<IProductRepository>(Data);
    });
}
public sealed class CatalogData(string scenario) : ISellerRepository, ICustomerRepository, IProductRepository
{
    public string? LastSearch { get; private set; }
    public List<Seller> Sellers { get; } = [
        new() { SellerId = 1, FirstName = "Ali", LastName = "Rezaei", Phone = "09120000001", City = "Tehran" },
        new() { SellerId = 2, FirstName = "Sara", LastName = "Mohammadi", City = "Shiraz" }];
    public List<Customer> Customers { get; } = [
        new() { CustomerId = 1, FullName = "Paya Energy Co.", Phone = "02144000001", City = "Tehran", Address = "Azadi Street" },
        new() { CustomerId = 2, FullName = "Novin Office Group", City = "Tehran" }];
    private void Check() { if (scenario == "failure") throw new InvalidOperationException("Private SQL diagnostic"); }
    Task<IReadOnlyList<Seller>> ISellerRepository.GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        Check(); LastSearch = search;
        return Task.FromResult<IReadOnlyList<Seller>>(Sellers.Where(x => search == null || (x.FirstName + " " + x.LastName).Contains(search, StringComparison.Ordinal)).ToList());
    }
    Task<IReadOnlyList<Customer>> ICustomerRepository.GetAllAsync(string? search, CancellationToken cancellationToken)
    {
        Check(); LastSearch = search;
        return Task.FromResult<IReadOnlyList<Customer>>(Customers.Where(x => search == null || x.FullName.Contains(search, StringComparison.Ordinal)).ToList());
    }
    Task<IReadOnlyList<Product>> IProductRepository.GetAllAsync(CancellationToken cancellationToken)
    {
        Check();
        return Task.FromResult<IReadOnlyList<Product>>(scenario == "empty" ? [] : [
            new() { ProductId = 1, ProductName = "UPS 1KVA", Category = "UPS", Unit = "Piece", LatestPrice = 79000000, LatestPriceStartDate = new DateOnly(2026, 1, 15) },
            new() { ProductId = 2, ProductName = "Battery Cabinet", Category = "Accessory", Unit = "Piece", LatestPrice = null }]);
    }
}
public sealed class CatalogTests
{
    private static readonly string[] Routes = ["/Sellers", "/Customers", "/Products"];
    private static async Task<HttpClient> SignedIn(CatalogFactory factory)
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
        using var response = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());
    }
    [Fact(DisplayName = "TC-06 | UC-03 | فهرست فروشنده‌ها")]
    public async Task TC06_SellersList()
    {
        using var factory = new CatalogFactory(); using var client = await SignedIn(factory);
        var html = await Page(client, "/Sellers");
        Assert.Contains("Rezaei", html); Assert.Contains("Mohammadi", html);
        Assert.Contains("09120000001", html); Assert.Contains("System Administrator", html);
    }
    [Fact(DisplayName = "TC-07 | UC-03 | جست‌وجوی نام فروشنده")]
    public async Task TC07_SellerSearch()
    {
        using var factory = new CatalogFactory(); using var client = await SignedIn(factory);
        var html = await Page(client, "/Sellers?search=%20Ali%20");
        Assert.Equal("Ali", factory.Data.LastSearch);
        Assert.Contains("Rezaei", html); Assert.DoesNotContain("Mohammadi", html);
        html = await Page(client, "/Sellers?search=%20%20");
        Assert.Null(factory.Data.LastSearch); Assert.Contains("Mohammadi", html);
    }
    [Fact(DisplayName = "TC-08 | UC-03 | جست‌وجوی بدون نتیجه")]
    public async Task TC08_SellerNotFound()
    {
        using var factory = new CatalogFactory(); using var client = await SignedIn(factory);
        var html = await Page(client, "/Sellers?search=__not_found__");
        Assert.Contains("موردی یافت نشد", html); Assert.DoesNotContain("<tbody>", html);
    }
    [Fact(DisplayName = "TC-09 | UC-04 | فهرست و جست‌وجوی خریداران")]
    public async Task TC09_CustomersListAndSearch()
    {
        using var factory = new CatalogFactory(); using var client = await SignedIn(factory);
        var html = await Page(client, "/Customers");
        Assert.Contains("Paya Energy Co.", html); Assert.Contains("Novin Office Group", html);
        html = await Page(client, "/Customers?search=Paya");
        Assert.Contains("Paya Energy Co.", html); Assert.DoesNotContain("Novin Office Group", html);
        Assert.Contains("Azadi Street", html);
    }
    [Fact(DisplayName = "TC-10 | UC-05 | محصولات و قیمت خروجی View")]
    public async Task TC10_ProductsPrices()
    {
        using var factory = new CatalogFactory(); using var client = await SignedIn(factory);
        var html = await Page(client, "/Products");
        Assert.Contains("UPS 1KVA", html); Assert.Contains("Piece", html);
        Assert.Contains("79,000,000", html); Assert.Contains("ریال", html);
    }
    [Fact(DisplayName = "TC-16-I2 | UC-03..05 | منع دسترسی بدون ورود")]
    public async Task TC16_ProtectedLists()
    {
        using var factory = new CatalogFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        foreach (var url in Routes)
        {
            using var response = await client.GetAsync(url);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/Auth/Login", response.Headers.Location!.OriginalString);
        }
    }
    [Fact(DisplayName = "TC-17-I2 | UC-03..05 | نگاشت خواندنی Viewها")]
    public void TC17_ViewMapping()
    {
        using var db = new SalesDashboardDbContext(new DbContextOptionsBuilder<SalesDashboardDbContext>()
            .UseSqlServer("Server=localhost;Database=WebAppDB;Integrated Security=True;TrustServerCertificate=True").Options);
        foreach (var entry in new[] { (typeof(Seller), "vw_Sellers"), (typeof(Customer), "vw_Customers"), (typeof(Product), "vw_Products") })
        {
            var entity = db.Model.FindEntityType(entry.Item1)!;
            Assert.Equal(entry.Item2, entity.GetViewName()); Assert.Equal("dbo", entity.GetViewSchema());
            Assert.Null(entity.FindPrimaryKey());
        }
        Assert.Contains("[dbo].[vw_Sellers]", db.Sellers.ToQueryString());
        Assert.Contains("[dbo].[vw_Customers]", db.Customers.ToQueryString());
        Assert.Contains("[dbo].[vw_Products]", db.Products.ToQueryString());
    }
    [Fact(DisplayName = "TC-18-I2 | UC-03..05 | نبود عملیات نوشتن")]
    public async Task TC18_NoWriteActions()
    {
        using var factory = new CatalogFactory(); using var client = await SignedIn(factory);
        foreach (var url in Routes)
        {
            var html = await Page(client, url);
            Assert.DoesNotContain("/Create", html); Assert.DoesNotContain("/Edit", html); Assert.DoesNotContain("/Delete", html);
            using var response = await client.PostAsync(url + "/Index", new FormUrlEncodedContent(new Dictionary<string, string>()));
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
    }
    [Fact(DisplayName = "EX-03 | UC-04 | خریدار پیدا نشد")]
    public async Task EX03_CustomerNotFound()
    {
        using var factory = new CatalogFactory(); using var client = await SignedIn(factory);
        Assert.Contains("موردی یافت نشد", await Page(client, "/Customers?search=__not_found__"));
    }
    [Fact(DisplayName = "EX-04 | UC-05 | محصول بی‌قیمت و فهرست خالی")]
    public async Task EX04_EmptyProductsAndMissingPrice()
    {
        using var normal = new CatalogFactory(); using var client = await SignedIn(normal);
        var html = await Page(client, "/Products");
        Assert.Contains("Battery Cabinet", html); Assert.Contains("ثبت نشده", html);
        using var empty = new CatalogFactory("empty"); using var emptyClient = await SignedIn(empty);
        Assert.Contains("موردی یافت نشد", await Page(emptyClient, "/Products"));
    }
    [Fact(DisplayName = "EX-05 | UC-03..05 | خطای منبع داده")]
    public async Task EX05_DataFailure()
    {
        using var factory = new CatalogFactory("failure"); using var client = await SignedIn(factory);
        foreach (var url in Routes)
        {
            using var response = await client.GetAsync(url);
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            var html = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain("Private SQL diagnostic", html);
            Assert.DoesNotContain("<tbody>", html);
        }
    }
}
