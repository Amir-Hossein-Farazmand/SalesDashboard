// Define the UserRepository contract without depending on EF Core or the web layer.
using SalesDashboard.Business.Models;
namespace SalesDashboard.Business.Interfaces;

public interface IUserRepository
{
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);
}