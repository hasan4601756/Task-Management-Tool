using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;

namespace TaskManagementTool.Application.Interfaces
{
    public interface IAccountService
    {
        Task<RegistrationResult> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
        Task<LoginResponseDto> RefreshAsync(string refreshToken);
    }
}