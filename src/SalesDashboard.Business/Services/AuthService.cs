// Validate the supplied credentials and active status, returning only non-secret identity data.
using SalesDashboard.Business.DTOs;
using SalesDashboard.Business.Interfaces;
namespace SalesDashboard.Business.Services;

public sealed class AuthService(IUserRepository userRepository) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    public async Task<AuthenticatedUserDto?> LoginAsync(string username, string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) return null;
        var user = await _userRepository.FindByUsernameAsync(username, cancellationToken);
        // The student-project policy compares passwords directly and rejects inactive users.
        if (user is null || !string.Equals(user.Password, password, StringComparison.Ordinal) || !user.IsActive)
            return null;
        return new AuthenticatedUserDto(user.UserId, user.Username, user.FullName);
    }
}