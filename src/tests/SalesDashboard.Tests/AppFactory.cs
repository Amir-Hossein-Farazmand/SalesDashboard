// Use a fake user repository by default; an explicit test SQL connection enables real user lookups.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
using SalesDashboard.Data;
namespace SalesDashboard.Tests;

public sealed class AppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connection = Environment.GetEnvironmentVariable("SALESDASHBOARD_TEST_SQL");
        builder.ConfigureServices(services =>
        {
            if (string.IsNullOrWhiteSpace(connection))
            {
                services.RemoveAll<IUserRepository>();
                services.AddScoped<IUserRepository, TestUserRepository>();
            }
            else
            {
                var parsed = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connection);
                if (!string.Equals(parsed.InitialCatalog, "WebAppDB", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Test connection must target WebAppDB.");
                services.RemoveAll<DbContextOptions<SalesDashboardDbContext>>();
                services.AddDbContext<SalesDashboardDbContext>(options => options.UseSqlServer(connection));
            }
        });
    }
}

internal sealed class TestUserRepository : IUserRepository
{
    public Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        User? user = username switch
        {
            "admin" => new User { UserId = 1, Username = "admin", Password = "Demo@123", FullName = "System Administrator", IsActive = true },
            "inactive" => new User { UserId = 2, Username = "inactive", Password = "Demo@123", FullName = "Inactive User", IsActive = false },
            _ => null
        };
        return Task.FromResult(user);
    }
}
