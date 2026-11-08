using Microsoft.EntityFrameworkCore;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Data;

public sealed class SalesDashboardDbContext(DbContextOptions<SalesDashboardDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Seller> Sellers => Set<Seller>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();
        user.ToTable("Users", "dbo");
        user.HasKey(x => x.UserId);
        user.Property(x => x.UserId).UseIdentityColumn();
        user.Property(x => x.Username).HasMaxLength(50).IsRequired();
        user.HasIndex(x => x.Username).IsUnique();
        user.Property(x => x.Password).HasMaxLength(50).IsRequired();
        user.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        user.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);
        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_Sellers", "dbo");
        });
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_Customers", "dbo");
        });
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_Products", "dbo");
            entity.Property(x => x.LatestPrice).HasPrecision(18, 0);
            entity.Property(x => x.LatestPriceStartDate).HasColumnType("date");
        });
    }
}