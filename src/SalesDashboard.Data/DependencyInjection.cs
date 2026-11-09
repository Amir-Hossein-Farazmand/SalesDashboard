// Register SQL Server repositories and enforce WebAppDB as the connection database.
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Data.Repositories;
namespace SalesDashboard.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
    {
        var parsed = new SqlConnectionStringBuilder(connectionString);
        if (!string.Equals(parsed.InitialCatalog, "WebAppDB", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("The application connection must target WebAppDB.");
        services.AddDbContext<SalesDashboardDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<ISalesRepository, SalesRepository>();
        return services;
    }
}