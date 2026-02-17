using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TaskManagementTool.API.Controllers;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.Tests.Controllers{
    public class TaskCategoryControllerTests
    {
        private readonly Mock<ITaskCategoryService> _serviceMock;
        private readonly Mock<ILogger<TaskCategoryController>> _loggerMock;
        private readonly TaskCategoryController _controller;

        public TaskCategoryControllerTests()
        {
            _serviceMock = new Mock<ITaskCategoryService>();
            _loggerMock = new Mock<ILogger<TaskCategoryController>>();

            _controller = new TaskCategoryController(
                _serviceMock.Object,
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

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            };
        }

        [Fact]
        public async Task GetAll_Should_Return_Ok_With_Categories()
        {
            var categories = new List<TaskCategoryDto>
            {
                new TaskCategoryDto { TaskCategoryId = 1, Name = "Dev" },
                new TaskCategoryDto { TaskCategoryId = 2, Name = "Test" }
            };

            _serviceMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(categories);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<TaskCategoryDto>>(okResult.Value);

            Assert.Equal(2, returned.Count());
        }

        [Fact]
        public async Task Create_Should_Return_Created_When_Success()
        {
            var dto = new TaskCategoryCreationDto { Name = "Dev" };

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ReturnsAsync(new ResponseDto { Succeeded = true });

            var result = await _controller.Create(dto);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task Create_Should_Return_Conflict_When_Failed()
        {
            var dto = new TaskCategoryCreationDto { Name = "Dev" };

            _serviceMock
                .Setup(x => x.AddAsync(dto))
                .ReturnsAsync(new ResponseDto
                {
                    Succeeded = false,
                    Errors = new[] { "Duplicate" }
                });

            var result = await _controller.Create(dto);

            Assert.IsType<ConflictResult>(result);
        }

        [Fact]
        public async Task Update_Should_Return_BadRequest_When_Failed()
        {
            var dto = new TaskCategoryDto { TaskCategoryId = 1, Name = "Updated" };

            _serviceMock
                .Setup(x => x.UpdateAsync(1, dto))
                .ReturnsAsync(new ResponseDto
                {
                    Succeeded = false,
                    Errors = new[] { "Error" }
                });

            var result = await _controller.Update(dto, 1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.NotNull(badRequest.Value);
        }

        [Fact]
        public async Task Delete_Should_Return_Ok_When_Success()
        {
            _serviceMock
                .Setup(x => x.RemoveAsync(1))
                .ReturnsAsync(new ResponseDto { Succeeded = true });

            var result = await _controller.Delete(1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_Should_Return_BadRequest_When_Failed()
        {
            _serviceMock
                .Setup(x => x.RemoveAsync(1))
                .ReturnsAsync(new ResponseDto
                {
                    Succeeded = false,
                    Errors = new[] { "Not found" }
                });

            var result = await _controller.Delete(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.NotNull(badRequest.Value);
        }
    }
}