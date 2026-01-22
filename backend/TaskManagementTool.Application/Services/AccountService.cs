using System.Security.Cryptography;
using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using System.Text;
namespace TaskManagementTool.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public AccountService(IIdentityRepository identityRepository, IJwtTokenService jwtTokenService, IRefreshTokenRepository refreshTokenRepo)
        {
            _identityRepository = identityRepository;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public static string ComputeTokenHash(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hashBytes = sha256.ComputeHash(bytes);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
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
                var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                var saved = await _refreshTokenRepo.AddAsync(ComputeTokenHash(refreshToken), dto.Email);
                if (!saved)
                    return new LoginResponseDto
                    {
                        Succeeded = false,
                        Errors = new[]{"Problem in saving refresh token."}
                    };
                else
                    return new LoginResponseDto
                    {
                        Succeeded = true,
                        Token = token,
                        RefreshToken = refreshToken
                    };
            }
            else
            {
                return response;
            }
        }

        public async Task<LoginResponseDto> RefreshAsync(string refreshToken)
        {
            var tokenHash = ComputeTokenHash(refreshToken);

            var storedToken = await _refreshTokenRepo.GetByTokenHashAsync(tokenHash);
            if (storedToken == null || !storedToken.IsActive)
                return new LoginResponseDto
                {
                    Succeeded = false,
                    Errors = new[]{"Incorrect Refresh token"}                    
                };

            await _refreshTokenRepo.RevokeAsync(storedToken);

            var user = await _identityRepository.FindByIdAsync(storedToken.UserId);
            if (user == null)
                return new LoginResponseDto
                {
                    Succeeded = false,
                    Errors = new[]{"The user with associated Refresh Token doesn't exists."}                    
                };

            var newJwt = await _jwtTokenService.GenerateTokenAsync(user.Email);
            var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var saved = await _refreshTokenRepo.AddAsync(ComputeTokenHash(newRefreshToken), user.Email);

            if (!saved)
                return new LoginResponseDto
                {
                    Succeeded = false,
                    Errors = new[]{"Problem in saving refresh token."}
                };

            return new LoginResponseDto
            {
                Succeeded = true,
                Token = newJwt,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var tokenHash = ComputeTokenHash(refreshToken);

            var storedToken = await _refreshTokenRepo
                .GetByTokenHashAsync(tokenHash);

            if (storedToken == null || !storedToken.IsActive)
                return false;

            await _refreshTokenRepo.RevokeAsync(storedToken);

            return true;
        }
        public async Task LogoutAllAsync(string userId)
        {
            await _refreshTokenRepo.RevokeAllForUserAsync(userId);
        }
    }
}