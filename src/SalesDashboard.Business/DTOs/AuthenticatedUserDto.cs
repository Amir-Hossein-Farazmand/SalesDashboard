namespace SalesDashboard.Business.DTOs;

public sealed record AuthenticatedUserDto(int UserId, string Username, string FullName);