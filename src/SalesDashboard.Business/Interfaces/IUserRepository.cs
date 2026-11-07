using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);
}