using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;

namespace TaskManagementTool.Application.Interfaces
{
    public interface IIdentityRepository
    {
        Task<RegistrationResult> CreateUserAsync(RegisterDto registerDto, string role = "User");
        Task<UserProfileDto?> FindByEmailAsync(string email);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}