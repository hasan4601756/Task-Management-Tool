using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementTool.Application;
using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Application.Services;
using TaskManagementTool.Domain.Entities;

namespace TaskManagementTool.Tests.Services{
    public class AccountServiceTests{ 
        private readonly Mock<IIdentityRepository> _identityRepoMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _identityRepoMock = new Mock<IIdentityRepository>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
            _loggerMock = new Mock<ILogger<AccountService>>();

            _accountService = new AccountService(_identityRepoMock.Object, _jwtTokenServiceMock.Object, _refreshTokenRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_Should_Return_Error_On_Invalid_Email()
        {
            // Arrange
            var dto = new RegisterDto()
            {
                Email = " ", 
                UserName = "TestUser",
                FullName = "Test User",
                Password = "TestUser@123",
                ConfirmPassword = "TestUser@123"
            };

            // Act
            var result = await _accountService.RegisterAsync(dto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains("Email is required.", result.Errors);

            // Ensure no repository calls were made
            _identityRepoMock.Verify(x => x.FindByEmailAsync(It.IsAny<string>()), Times.Never);
            _identityRepoMock.Verify(x => x.FindByUsernameAsync(It.IsAny<string>()), Times.Never);
            _identityRepoMock.Verify(x => x.CreateUserAsync(It.IsAny<RegisterDto>(), "User"), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_Should_Return_Error_When_Passwords_Do_Not_Match()
        {
            var dto = new RegisterDto
            {
                Email = "test@test.com",
                UserName = "TestUser",
                FullName = "Test User",
                Password = "TestUser@123",
                ConfirmPassword = "DifferentPassword@123"
            };

            var result = await _accountService.RegisterAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains("Passwords do not match.", result.Errors);

            _identityRepoMock.Verify(x =>
                x.FindByEmailAsync(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_Should_Return_Error_When_Password_Is_Too_Short()
        {
            var dto = new RegisterDto
            {
                Email = "test@test.com",
                UserName = "TestUser",
                FullName = "Test User",
                Password = "T@1a",
                ConfirmPassword = "T@1a"
            };

            var result = await _accountService.RegisterAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains("Password must be at least 6 characters long.", result.Errors);
        }

        [Fact]
        public async Task RegisterAsync_Should_Return_Error_When_Email_Already_Exists()
        {
            var dto = new RegisterDto
            {
                Email = "test@test.com",
                UserName = "TestUser",
                FullName = "Test User",
                Password = "TestUser@123",
                ConfirmPassword = "TestUser@123"
            };

            _identityRepoMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(new UserProfileDto()); // simulate existing user

            var result = await _accountService.RegisterAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains("Registration failed. Please verify your details.", result.Errors);

            _identityRepoMock.Verify(x =>
                x.CreateUserAsync(It.IsAny<RegisterDto>(), "User"),
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_Should_Return_Error_When_Username_Already_Exists()
        {
            var dto = new RegisterDto
            {
                Email = "test@test.com",
                UserName = "TestUser",
                FullName = "Test User",
                Password = "TestUser@123",
                ConfirmPassword = "TestUser@123"
            };

            // _identityRepoMock
            //     .Setup(x => x.FindByEmailAsync(dto.Email))
            //     .ReturnsAsync((UserProfileDto?)null);

            _identityRepoMock
                .Setup(x => x.FindByUsernameAsync(dto.UserName))
                .ReturnsAsync(new UserProfileDto());

            var result = await _accountService.RegisterAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains("Username is already registered.", result.Errors);

            _identityRepoMock.Verify(x =>
                x.CreateUserAsync(It.IsAny<RegisterDto>(), "User"),
                Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_Should_Return_Success_When_Data_Is_Valid()
        {
            var dto = new RegisterDto
            {
                Email = "test@test.com",
                UserName = "TestUser",
                FullName = "Test User",
                Password = "TestUser@123",
                ConfirmPassword = "TestUser@123"
            };

            _identityRepoMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((UserProfileDto?)null);

            _identityRepoMock
                .Setup(x => x.FindByUsernameAsync(dto.UserName))
                .ReturnsAsync((UserProfileDto?)null);

            _identityRepoMock
                .Setup(x => x.CreateUserAsync(dto, "User"))
                .ReturnsAsync(new RegistrationResult
                {
                    Succeeded = true,
                    Errors = Array.Empty<string>()
                });

            var result = await _accountService.RegisterAsync(dto);

            Assert.True(result.Succeeded);
            Assert.Empty(result.Errors);

            _identityRepoMock.Verify(x =>
                x.CreateUserAsync(dto, "User"),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_Should_Return_Error_On_Invalid_Email()
        {
            var dto = new LoginRequestDto
            {
                Email = "Invalid Email",
                Password = "Password@123",
                RememberMe = false
            };

            _identityRepoMock
            .Setup(x => x.LoginAsync(dto))
            .ReturnsAsync(new LoginResponseDto
            {
                Succeeded = false,
                Errors = new String[] {"Invalid Email Format."}
            });

            var result = await _accountService.LoginAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains("Invalid Email Format.", result.Errors);
        }

        [Fact]
        public async Task LoginAsync_Should_Return_Error_On_Invalid_Password()
        {
            var dto = new LoginRequestDto
            {
                Email = "email@gmail.com",
                Password = "InvalidPassword",
                RememberMe = false
            };

            _identityRepoMock
            .Setup(x => x.LoginAsync(dto))
            .ReturnsAsync(new LoginResponseDto
            {
                Succeeded = false,
                Errors = new String[] {"Invalid email or password."}
            });

            var result = await _accountService.LoginAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains("Invalid email or password.", result.Errors);
        }

        [Fact]
        public async Task RefreshAsync_Should_Return_Error_When_Token_Not_Found()
        {
            var refreshToken = "test-refresh-token";
            var tokenHash = AccountService.ComputeTokenHash(refreshToken);

            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenHashAsync(tokenHash))
                .ReturnsAsync((RefreshToken?)null);

            var result = await _accountService.RefreshAsync(refreshToken);

            Assert.False(result.Succeeded);
            Assert.Contains("Incorrect Refresh token", result.Errors);

            _refreshTokenRepoMock.Verify(x =>
                x.GetByTokenHashAsync(tokenHash),
                Times.Once);
        }

        [Fact]
        public async Task RefreshAsync_Should_Return_Error_When_Token_Is_Inactive()
        {
            var refreshToken = "test-refresh-token";
            var tokenHash = AccountService.ComputeTokenHash(refreshToken);

            var storedToken = new RefreshToken
            {
                TokenHash = tokenHash
            };

            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenHashAsync(tokenHash))
                .ReturnsAsync(storedToken);

            var result = await _accountService.RefreshAsync(refreshToken);

            Assert.False(result.Succeeded);
            Assert.Contains("Incorrect Refresh token", result.Errors);

            _refreshTokenRepoMock.Verify(x =>
                x.RevokeAsync(It.IsAny<RefreshToken>()),
                Times.Never);
        }

        [Fact]
        public async Task RefreshAsync_Should_Return_Success_When_Data_Is_Valid()
        {
            var refreshToken = "test-refresh-token";
            var tokenHash = AccountService.ComputeTokenHash(refreshToken);

            var storedToken = new RefreshToken
            {
                TokenHash = tokenHash,
                UserId = "user-id",
                RevokedAt=null,
                ExpiresAt=DateTime.UtcNow.AddDays(6)
            };

            var user = new UserProfileDto
            {
                Email = "test@test.com"
            };

            _refreshTokenRepoMock
            .Setup(x => x.GetByTokenHashAsync(It.IsAny<string>()))
            .ReturnsAsync(storedToken);

            _identityRepoMock
                .Setup(x => x.FindByIdAsync(storedToken.UserId))
                .ReturnsAsync(user);

            _jwtTokenServiceMock
                .Setup(x => x.GenerateTokenAsync(user.Email))
                .ReturnsAsync("new-jwt-token");

            _refreshTokenRepoMock
                .Setup(x => x.AddAsync(It.IsAny<string>(), user.Email))
                .ReturnsAsync(true);

            var result = await _accountService.RefreshAsync(refreshToken);

            Assert.True(result.Succeeded);
            Assert.Equal("new-jwt-token", result.Token);
            Assert.False(string.IsNullOrEmpty(result.RefreshToken));

            _refreshTokenRepoMock.Verify(x =>
                x.RevokeAsync(storedToken),
                Times.Once);

            _refreshTokenRepoMock.Verify(x =>
                x.AddAsync(It.IsAny<string>(), user.Email),
                Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_Should_Return_False_When_Token_Not_Found()
        {
            var refreshToken = "invalid-token";

            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenHashAsync(It.IsAny<string>()))
                .ReturnsAsync((RefreshToken?)null);

            var result = await _accountService.LogoutAsync(refreshToken);

            Assert.False(result);

            _refreshTokenRepoMock.Verify(x =>
                x.RevokeAsync(It.IsAny<RefreshToken>()),
                Times.Never);
        }

        [Fact]
        public async Task LogoutAsync_Should_Return_False_When_Token_Is_Inactive()
        {
            var storedToken = new RefreshToken
            {
                RevokedAt = DateTime.UtcNow.AddDays(-1)
            };

            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenHashAsync(It.IsAny<string>()))
                .ReturnsAsync(storedToken);

            var result = await _accountService.LogoutAsync("token");

            Assert.False(result);

            _refreshTokenRepoMock.Verify(x =>
                x.RevokeAsync(It.IsAny<RefreshToken>()),
                Times.Never);
        }

        [Fact]
        public async Task LogoutAsync_Should_Revoke_Token_And_Return_True()
        {
            var storedToken = new RefreshToken
            {
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                RevokedAt = null
            };

            _refreshTokenRepoMock
                .Setup(x => x.GetByTokenHashAsync(It.IsAny<string>()))
                .ReturnsAsync(storedToken);

            var result = await _accountService.LogoutAsync("valid-token");

            Assert.True(result);

            _refreshTokenRepoMock.Verify(x =>
                x.RevokeAsync(storedToken),
                Times.Once);
        }

        [Fact]
        public async Task LogoutAllAsync_Should_Revoke_All_User_Tokens()
        {
            var userId = "user-id";

            await _accountService.LogoutAllAsync(userId);

            _refreshTokenRepoMock.Verify(x =>
                x.RevokeAllForUserAsync(userId),
                Times.Once);
        }

        [Fact]
        public async Task GetUserProfileAsync_Should_Return_User_When_Found()
        {
            var userId = "user-id";

            var user = new UserProfileDto
            {
                Email = "test@test.com"
            };

            _identityRepoMock
                .Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);

            var result = await _accountService.GetUserProfileAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserProfileAsync_Should_Return_Null_When_Not_Found()
        {
            _identityRepoMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((UserProfileDto?)null);

            var result = await _accountService.GetUserProfileAsync("id");

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_Should_Return_Error_When_User_Not_Found()
        {
            _identityRepoMock
                .Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((UserProfileDto?)null);

            var result = await _accountService.UpdateUserProfileAsync("id", new UserProfileDto());

            Assert.False(result.Succeeded);
            Assert.Contains("User doesn't exists", result.Errors);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_Should_Return_Error_When_Email_Already_Registered()
        {
            var existingUser = new UserProfileDto
            {
                Email = "old@test.com"
            };

            var dto = new UserProfileDto
            {
                Email = "new@test.com"
            };

            _identityRepoMock
                .Setup(x => x.FindByIdAsync("id"))
                .ReturnsAsync(existingUser);

            _identityRepoMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(new UserProfileDto());

            var result = await _accountService.UpdateUserProfileAsync("id", dto);

            Assert.False(result.Succeeded);
            Assert.Contains("Email is already registered.", result.Errors);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_Should_Update_Profile_When_Valid()
        {
            var existingUser = new UserProfileDto
            {
                Email = "old@test.com"
            };

            var dto = new UserProfileDto
            {
                Email = "old@test.com"
            };

            _identityRepoMock
                .Setup(x => x.FindByIdAsync("id"))
                .ReturnsAsync(existingUser);

            _identityRepoMock
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((UserProfileDto?)null);

            _identityRepoMock
                .Setup(x => x.UpdateUserProfile(existingUser.Email, dto))
                .ReturnsAsync(new ResponseDto { Succeeded = true });

            var result = await _accountService.UpdateUserProfileAsync("id", dto);

            Assert.True(result.Succeeded);

            _identityRepoMock.Verify(x =>
                x.UpdateUserProfile(existingUser.Email, dto),
                Times.Once);
        }

        [Fact]
        public async Task DeleteUserProfile_Should_Return_Error_When_UserId_Is_Missing()
        {
            var result = await _accountService.DeleteUserProfile(null, null, false);

            Assert.False(result.Succeeded);
            Assert.Contains("Authenticated user id is missing.", result.Errors);
        }
    }
}