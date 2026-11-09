// Carry the authenticated user identity to the presentation layer without the password.
namespace SalesDashboard.Business.DTOs;

public sealed record AuthenticatedUserDto(int UserId, string Username, string FullName);