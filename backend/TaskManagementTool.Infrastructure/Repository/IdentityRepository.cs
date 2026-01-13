using TaskManagementTool.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.Application.Services
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<RegistrationResult> CreateUserAsync(RegisterDto dto, string role = "user")
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                throw new Exception("User creation failed");

            await _userManager.AddToRoleAsync(user, role);

            return new RegistrationResult
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    }
}