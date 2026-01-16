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
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<RegistrationResult> CreateUserAsync(RegisterDto dto, string roleName = "User")
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return new RegistrationResult
                {
                    Succeeded = result.Succeeded,
                    Errors = result.Errors.Select(e => e.Description)
                };
            
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null) { throw new Exception($"Role '{roleName}' does not exist."); }

            await _userManager.AddToRoleAsync(user, role.Name);

            return new RegistrationResult
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserProfileDto?> FindByEmailAsync(string email)
        {
            ApplicationUser? appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) return null;
            else
            {
                return new UserProfileDto
                {
                    UserName = appUser.UserName,
                    Email = appUser.Email,
                    FullName = appUser.FullName,
                    PhoneNumber = appUser.PhoneNumber
                };
            }
        }
    }
}