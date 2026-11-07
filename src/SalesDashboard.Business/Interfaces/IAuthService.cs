using SalesDashboard.Business.DTOs;
namespace SalesDashboard.Business.Interfaces;

public interface IAuthService
{
    Task<AuthenticatedUserDto?> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}