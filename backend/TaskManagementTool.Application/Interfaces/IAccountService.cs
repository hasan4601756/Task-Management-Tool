using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;

namespace TaskManagementTool.Application.Interfaces
{
    public interface IAccountService
    {
        Task<RegistrationResult> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto> RefreshAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task LogoutAllAsync(string userId);
        Task<UserProfileDto?> GetUserProfileAsync(string Id);
        Task<ResponseDto> UpdateUserProfileAsync(string userId, UserProfileDto dto);

        Task<ResponseDto> DeleteUserProfile(string? userId, string? routeId, bool isAdmin);
    }
}