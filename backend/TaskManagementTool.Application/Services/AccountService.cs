using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
namespace TaskManagementTool.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public AccountService(IIdentityRepository identityRepository, IJwtTokenService jwtTokenService)
        {
            _identityRepository = identityRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Email))
                errors.Add("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required.");

            if (dto.Password != dto.ConfirmPassword)
                errors.Add("Passwords do not match.");

            if (dto.Password.Length < 6)
                errors.Add("Password must be at least 6 characters long.");

            if (!dto.Password.Any(char.IsUpper))
                errors.Add("Password must contain at least one uppercase letter.");

            if (!dto.Password.Any(char.IsLower))
                errors.Add("Password must contain at least one lowercase letter.");

            if (!dto.Password.Any(char.IsDigit))
                errors.Add("Password must contain at least one digit.");

            if (!dto.Password.Any(ch => !char.IsLetterOrDigit(ch)))
                errors.Add("Password must contain at least one non-alphanumeric character.");        

            if (errors.Any())
            {
                return new RegistrationResult
                {
                    Succeeded = false,
                    Errors = errors
                };
            }

            if (await _identityRepository.FindByEmailAsync(dto.Email) != null)
            {
                return new RegistrationResult
                {
                    Succeeded = false,
                    Errors = new[] { "Email is already registered." }
                };
            }

            return await _identityRepository.CreateUserAsync(dto);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var response = await _identityRepository.LoginAsync(dto);

            if (response.Succeeded)
            {
                var token = await _jwtTokenService.GenerateTokenAsync(dto.Email);

                return new LoginResponseDto
                {
                    Succeeded = true,
                    Token = token
                };
            }
            else
            {
                return response;
            }
        }
    }
}