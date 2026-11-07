using Microsoft.EntityFrameworkCore;
using SalesDashboard.Business.Interfaces;
using SalesDashboard.Business.Models;
namespace SalesDashboard.Data.Repositories;

public sealed class UserRepository(SalesDashboardDbContext context) : IUserRepository
{
    public Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Username == username, cancellationToken);
}