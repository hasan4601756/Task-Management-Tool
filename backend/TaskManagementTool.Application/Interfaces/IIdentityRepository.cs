using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;

namespace TaskManagementTool.Application.Interfaces
{
    public interface IIdentityRepository
    {
        Task<RegistrationResult> CreateUserAsync(RegisterDto registerDto, string role = "User");
        Task<UserProfileDto?> FindByEmailAsync(string email);
        Task<UserProfileDto?> FindByIdAsync(string id);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<IEnumerable<UserDto>> GetAllUsers(); 
        Task<ResponseDto> UpdateUserProfile(string email, UserProfileDto dto);
        Task<ResponseDto> DeleteUserAsync(string userId);
    }
}