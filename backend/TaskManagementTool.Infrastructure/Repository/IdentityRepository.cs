using TaskManagementTool.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Application;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementTool.Infrastructure.Repository
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
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

        public async Task<UserProfileDto?> FindByIdAsync(string id)
        {
            ApplicationUser? appUser = await _userManager.FindByIdAsync(id);
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

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new LoginResponseDto
                {
                    Succeeded = false,
                    Errors = new[] { "Invalid Email." }
                };
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                dto.Password,
                dto.RememberMe,
                lockoutOnFailure: true
            );

            if (!result.Succeeded)
            {
                return new LoginResponseDto
                {
                    Succeeded = false,
                    Errors = new[] { "Invalid email or password." }
                };
            }
            return new LoginResponseDto
            {
                Succeeded = true,
                Errors = null
            }; 
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users =  await _userManager.Users.ToListAsync();

            var userdtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userdto = new UserDto()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email
                };
                userdtos.Add(userdto);
            }

            return userdtos;
        }
    }
}