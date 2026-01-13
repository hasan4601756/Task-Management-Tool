using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;

namespace TaskManagementTool.Application.Interfaces{
    public interface IIdentityRepository
    {
        public Task<RegistrationResult> CreateUserAsync(RegisterDto registerDto, string role = "User");
    }
}