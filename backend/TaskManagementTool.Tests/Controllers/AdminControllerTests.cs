using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TaskManagementTool.API.Controllers;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IAdminService> _adminServiceMock;
        private readonly Mock<ILogger<AdminController>> _loggerMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _adminServiceMock = new Mock<IAdminService>();
            _loggerMock = new Mock<ILogger<AdminController>>();

            _controller = new AdminController(
                _adminServiceMock.Object,
                _loggerMock.Object);

            SetAdminUser();
        }

        private void SetAdminUser()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "admin1"),
                new Claim(ClaimTypes.Role, "Admin")
            };

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
    public async Task GetAllUsers_Should_Return_Ok_With_Users()
    {
        var users = new List<UserDto>
        {
            new UserDto { UserId = "1", UserName = "User1" },
            new UserDto { UserId = "2", UserName = "User2" }
        };

        _adminServiceMock
            .Setup(x => x.GetAllUsers())
            .ReturnsAsync(users);

        var result = await _controller.GetAllUsers();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);

        Assert.Equal(2, returnedUsers.Count());
    }

    [Fact]
    public async Task GetAllTasks_Should_Return_Ok_With_Tasks()
    {
        var tasks = new List<TaskDto>
        {
            new TaskDto { Id = 1, Title = "Task1" },
            new TaskDto { Id = 2, Title = "Task2" }
        };

        _adminServiceMock
            .Setup(x => x.GetAllTasks())
            .ReturnsAsync(tasks);

        var result = await _controller.GetAllTasks();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskDto>>(okResult.Value);

        Assert.Equal(2, returnedTasks.Count());
    }

        [Fact]
        public async Task AssignTask_Should_Return_Ok_When_Success()
        {
            var response = new ResponseDto { Succeeded = true };

            _adminServiceMock
                .Setup(x => x.AssignTask("user1", 10))
                .ReturnsAsync(response);

            var result = await _controller.AssignTask("user1", 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var value = okResult.Value;
            Assert.NotNull(value);

            var succeededProp = value.GetType().GetProperty("Succeeded");
            Assert.True((bool)succeededProp.GetValue(value));
        }

        [Fact]
        public async Task AssignTask_Should_Return_BadRequest_When_Failed()
        {
            var response = new ResponseDto
            {
                Succeeded = false,
                Errors = new[] { "Task not found" }
            };

            _adminServiceMock
                .Setup(x => x.AssignTask("user1", 10))
                .ReturnsAsync(response);

            var result = await _controller.AssignTask("user1", 10);

            var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            var value = badResult.Value;
            var succeededProp = value.GetType().GetProperty("Succeeded");

            Assert.False((bool)succeededProp.GetValue(value));
        }
    }
}
