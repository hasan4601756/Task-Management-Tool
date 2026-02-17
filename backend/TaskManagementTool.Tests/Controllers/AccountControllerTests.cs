using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementTool.API.Controllers;
using TaskManagementTool.Application;
using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly AccountController _controller;
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<ILogger<AccountController>> _loggerMock;

        public AccountControllerTests()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _loggerMock = new Mock<ILogger<AccountController>>();

            _controller = new AccountController(_accountServiceMock.Object, _loggerMock.Object);
        }

        private void SetUser(string userId, params string[] roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };
        }

        [Fact]
        public void GetUserRole_Should_Return_User_Roles()
        {
            SetUser("user1", "Admin", "User");

            var result = _controller.GetUserRole() as OkObjectResult;

            Assert.NotNull(result);

            var roles = Assert.IsType<List<string>>(result.Value);
            Assert.Contains("Admin", roles);
            Assert.Contains("User", roles);
        }

        [Fact]
        public async Task Register_Should_Return_BadRequest_When_Failed()
        {
            SetUser("admin1", "Admin");

            var dto = new RegisterDto { UserName = "test" };

            _accountServiceMock
                .Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(new RegistrationResult
                {
                    Succeeded = false,
                    Errors = new[] { "error" }
                });

            var result = await _controller.Register(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_Should_Return_Ok_When_Success()
        {
            SetUser("admin1", "Admin");

            var dto = new RegisterDto { UserName = "test" };

            _accountServiceMock
                .Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(new RegistrationResult { Succeeded = true });

            var result = await _controller.Register(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully", ok.Value);
        }

        [Fact]
        public async Task Login_Should_Return_Unauthorized_When_Failed()
        {
            var dto = new LoginRequestDto { Email = "a@test.com" };

            _accountServiceMock
                .Setup(x => x.LoginAsync(dto))
                .ReturnsAsync(new LoginResponseDto
                {
                    Succeeded = false,
                    Errors = new[] { "error" }
                });

            var result = await _controller.Login(dto);

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public async Task Login_Should_Return_Ok_When_Success()
        {
            var dto = new LoginRequestDto { Email = "a@test.com" };

            var response = new LoginResponseDto { Succeeded = true };

            _accountServiceMock
                .Setup(x => x.LoginAsync(dto))
                .ReturnsAsync(response);

            var result = await _controller.Login(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, ok.Value);
        }


        [Fact]
        public async Task Refresh_Should_Return_Unauthorized_When_No_Token()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.Refresh();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Refresh_Should_Return_Unauthorized_When_Invalid()
        {
            var cookiesMock = new Mock<IRequestCookieCollection>();

            cookiesMock
                .Setup(c => c["refreshToken"])
                .Returns("token");

            var context = new DefaultHttpContext();
            context.Request.Cookies = cookiesMock.Object;

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            _accountServiceMock
                .Setup(x => x.RefreshAsync("token"))
                .ReturnsAsync(new LoginResponseDto
                {
                    Succeeded = false,
                    Errors = new[] { "invalid" }
                });

            var result = await _controller.Refresh();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }


        [Fact]
        public async Task Logout_Should_Return_Unauthorized_When_No_Token()
        {
            SetUser("user1");

            var result = await _controller.Logout();

            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Logout_Should_Return_NoContent_When_Success()
        {
            // Arrange
            var cookiesMock = new Mock<IRequestCookieCollection>();
            cookiesMock.Setup(c => c["refreshToken"]).Returns("token");

            var context = new DefaultHttpContext();
            context.Request.Cookies = cookiesMock.Object;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            };

            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "TestAuth"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            _accountServiceMock
                .Setup(x => x.LogoutAsync("token"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Logout();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UserProfile_Should_Return_NotFound_When_UserId_Null()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _controller.UserProfile();

            Assert.IsType<NotFoundResult>(result.Result);
        }


        [Fact]
        public async Task UserProfile_Should_Return_Ok_When_Found()
        {
            SetUser("user1");

            var profile = new UserProfileDto { UserName = "test" };

            _accountServiceMock
                .Setup(x => x.GetUserProfileAsync("user1"))
                .ReturnsAsync(profile);

            var result = await _controller.UserProfile();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(profile, ok.Value);
        }


        [Fact]
        public async Task DeleteProfile_Should_Return_BadRequest_When_Failed()
        {
            SetUser("user1");

            _accountServiceMock
                .Setup(x => x.DeleteUserProfile("user1", null, false))
                .ReturnsAsync(new ResponseDto
                {
                    Succeeded = false,
                    Errors = new[] { "error" }
                });

            var result = await _controller.DeleteProfile(null);

            Assert.IsType<BadRequestResult>(result.Result);
        }


        [Fact]
        public async Task DeleteProfile_Should_Return_Ok_When_Success()
        {
            SetUser("user1");

            var response = new ResponseDto { Succeeded = true };

            _accountServiceMock
                .Setup(x => x.DeleteUserProfile("user1", null, false))
                .ReturnsAsync(response);

            var result = await _controller.DeleteProfile(null);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response, ok.Value);
        }
    }
}