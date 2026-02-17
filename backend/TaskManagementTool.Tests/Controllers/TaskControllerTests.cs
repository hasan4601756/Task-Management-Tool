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
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly Mock<ILogger<TaskController>> _loggerMock;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _loggerMock = new Mock<ILogger<TaskController>>();

            _controller = new TaskController(
                _taskServiceMock.Object,
                _loggerMock.Object);
        }

        private void SetUser(string userId, bool isAdmin = false)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var identity = new ClaimsIdentity(claims, "TestAuth");

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };
        }

        [Fact]
        public async Task Dashboard_Should_Return_Ok()
        {
            SetUser("user1");

            var dashboard = new DashboardDto
            {
                CompletedTasks = 1,
                InProgressTasks = 2,
                PendingTasks = 3
            };

            _taskServiceMock
                .Setup(x => x.GetDashboardAsync("user1", false))
                .ReturnsAsync(dashboard);

            var result = await _controller.Dashboard();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dashboard, ok.Value);
        }

        [Fact]
        public async Task GetAll_Should_Return_Ok()
        {
            SetUser("user1");

            var tasks = new List<TaskDto>
            {
                new TaskDto { Id = 1, Title = "Task1" }
            };

            _taskServiceMock
                .Setup(x => x.GetAllAsync("user1"))
                .ReturnsAsync(tasks);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(tasks, ok.Value);
        }

        [Fact]
        public async Task GetDetail_Should_Return_NotFound_When_Null()
        {
            SetUser("user1");

            _taskServiceMock
                .Setup(x => x.GetAsync(1, "user1"))
                .ReturnsAsync((TaskDetailDto?)null);

            var result = await _controller.GetDetail(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Create_Should_Return_Ok_When_Success()
        {
            SetUser("user1");

            var dto = new TaskCreationDto { Title = "New Task" };

            _taskServiceMock
                .Setup(x => x.AddAsync(dto, "user1"))
                .ReturnsAsync(new ResponseDto { Succeeded = true });

            var result = await _controller.Create(dto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Create_Should_Return_BadRequest_When_Failed()
        {
            SetUser("user1");

            var dto = new TaskCreationDto();

            _taskServiceMock
                .Setup(x => x.AddAsync(dto, "user1"))
                .ReturnsAsync(new ResponseDto { Succeeded = false });

            var result = await _controller.Create(dto);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task Update_Should_Return_Ok_When_Success()
        {
            SetUser("user1");

            var dto = new TaskUpdationDto();

            _taskServiceMock
                .Setup(x => x.UpdateAsync(1, dto, "user1", false))
                .ReturnsAsync(new ResponseDto { Succeeded = true });

            var result = await _controller.Update(1, dto);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Update_Should_Return_BadRequest_When_Failed()
        {
            SetUser("user1");

            var dto = new TaskUpdationDto();

            _taskServiceMock
                .Setup(x => x.UpdateAsync(1, dto, "user1", false))
                .ReturnsAsync(new ResponseDto { Succeeded = false });

            var result = await _controller.Update(1, dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task Delete_Should_Return_Ok_When_Success()
        {
            SetUser("user1");

            _taskServiceMock
                .Setup(x => x.RemoveAsync(1, "user1", false))
                .ReturnsAsync(new ResponseDto { Succeeded = true });

            var result = await _controller.Delete(1);

            Assert.IsType<OkResult>(result);
        }


        [Fact]
        public async Task Delete_Should_Return_BadRequest_When_Failed()
        {
            SetUser("user1");

            _taskServiceMock
                .Setup(x => x.RemoveAsync(1, "user1", false))
                .ReturnsAsync(new ResponseDto { Succeeded = false });

            var result = await _controller.Delete(1);

            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task Update_As_Admin_Should_Set_IsAdmin_True()
        {
            SetUser("admin1", isAdmin: true);

            var dto = new TaskUpdationDto();

            _taskServiceMock
                .Setup(x => x.UpdateAsync(1, dto, "admin1", true))
                .ReturnsAsync(new ResponseDto { Succeeded = true });

            var result = await _controller.Update(1, dto);

            Assert.IsType<OkResult>(result);
        }


        [Fact]
        public async Task Dashboard_Should_Throw_When_UserId_Missing()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _controller.Dashboard());
        }
    }
}
