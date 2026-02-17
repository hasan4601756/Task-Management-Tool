using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Application.Services;
using TaskManagementTool.Domain.Entities;

namespace TaskManagementTool.Tests.Services
{
    public class TaskCategoryServiceTests
    {
        private readonly TaskCategoryService _categoryService;
        private readonly Mock<ILogger<TaskCategoryService>> _loggerMock;
        private readonly Mock<ITaskCategoryRepository> _categoryRepoMock;

        public TaskCategoryServiceTests()
        {
            _loggerMock = new Mock<ILogger<TaskCategoryService>>();
            _categoryRepoMock = new Mock<ITaskCategoryRepository>();

            _categoryService = new TaskCategoryService(_categoryRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Error_When_Name_Already_Exists()
        {
            var dto = new TaskCategoryCreationDto
            {
                Name = "Development",
                Description = "Dev tasks"
            };

            _categoryRepoMock
                .Setup(x => x.GetCategoryByName(dto.Name))
                .ReturnsAsync(new TaskCategory());

            var result = await _categoryService.AddAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains("Category with same name already exists.", result.Errors);

            _categoryRepoMock.Verify(x => x.CreateCategory(It.IsAny<TaskCategory>()), Times.Never);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Success_When_Create_Succeeds()
        {
            var dto = new TaskCategoryCreationDto
            {
                Name = "Development",
                Description = "Dev tasks"
            };

            _categoryRepoMock
                .Setup(x => x.GetCategoryByName(dto.Name))
                .ReturnsAsync((TaskCategory?)null);

            _categoryRepoMock
                .Setup(x => x.CreateCategory(It.IsAny<TaskCategory>()))
                .ReturnsAsync(true);

            var result = await _categoryService.AddAsync(dto);

            Assert.True(result.Succeeded);
            Assert.Null(result.Errors);

            _categoryRepoMock.Verify(x => x.CreateCategory(
                It.Is<TaskCategory>(c =>
                    c.Name == dto.Name &&
                    c.Description == dto.Description)),
                Times.Once);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Error_When_Create_Fails()
        {
            var dto = new TaskCategoryCreationDto
            {
                Name = "Development",
                Description = "Dev tasks"
            };

            _categoryRepoMock
                .Setup(x => x.GetCategoryByName(dto.Name))
                .ReturnsAsync((TaskCategory?)null);

            _categoryRepoMock
                .Setup(x => x.CreateCategory(It.IsAny<TaskCategory>()))
                .ReturnsAsync(false);

            var result = await _categoryService.AddAsync(dto);

            Assert.False(result.Succeeded);
            Assert.Contains("An unknown Error occurred.", result.Errors);
        }


        [Fact]
        public async Task GetAllAsync_Should_Return_Mapped_Categories()
        {
            var categories = new List<TaskCategory>
            {
                new TaskCategory
                {
                    TaskCategoryId = 1,
                    Name = "Dev",
                    Description = "Development"
                },
                new TaskCategory
                {
                    TaskCategoryId = 2,
                    Name = "QA",
                    Description = "Testing"
                }
            };

            _categoryRepoMock
                .Setup(x => x.GetCategories())
                .ReturnsAsync(categories);

            var result = await _categoryService.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Dev");
            Assert.Contains(result, c => c.TaskCategoryId == 2);
        }


        [Fact]
        public async Task GetAllAsync_Should_Log_And_Throw_When_Exception_Occurs()
        {
            var exception = new Exception("DB Failure");

            _categoryRepoMock
                .Setup(x => x.GetCategories())
                .ThrowsAsync(exception);

            await Assert.ThrowsAsync<Exception>(() => _categoryService.GetAllAsync());

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString().Contains("GetAllAsync failed")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }


        [Fact]
        public async Task GetAsync_Should_Return_Null_When_Not_Found()
        {
            _categoryRepoMock
                .Setup(x => x.GetCategory(1))
                .ReturnsAsync((TaskCategory?)null);

            var result = await _categoryService.GetAsync(1);

            Assert.Null(result);
        }


        [Fact]
        public async Task GetAsync_Should_Return_Mapped_Category()
        {
            var category = new TaskCategory
            {
                TaskCategoryId = 1,
                Name = "Dev",
                Description = "Development"
            };

            _categoryRepoMock
                .Setup(x => x.GetCategory(1))
                .ReturnsAsync(category);

            var result = await _categoryService.GetAsync(1);

            Assert.NotNull(result);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.TaskCategoryId, result.TaskCategoryId);
        }

        [Fact]
        public async Task GetAsync_Should_Log_And_Throw_When_Exception_Occurs()
        {
            var exception = new Exception("Failure");

            _categoryRepoMock
                .Setup(x => x.GetCategory(It.IsAny<int>()))
                .ThrowsAsync(exception);

            await Assert.ThrowsAsync<Exception>(() => _categoryService.GetAsync(1));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString().Contains("GetAsync failed")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }


        [Fact]
        public async Task RemoveAsync_Should_Return_Error_When_Not_Found()
        {
            _categoryRepoMock
                .Setup(x => x.GetCategory(1))
                .ReturnsAsync((TaskCategory?)null);

            var result = await _categoryService.RemoveAsync(1);

            Assert.False(result.Succeeded);
            Assert.Contains("No category with given id exists.", result.Errors);
        }


        [Fact]
        public async Task RemoveAsync_Should_Return_Error_When_Delete_Fails()
        {
            var category = new TaskCategory();

            _categoryRepoMock
                .Setup(x => x.GetCategory(1))
                .ReturnsAsync(category);

            _categoryRepoMock
                .Setup(x => x.DeleteCategory(category))
                .ReturnsAsync(false);

            var result = await _categoryService.RemoveAsync(1);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Error_When_Already_Exists()
        {
            var dto = new TaskCategoryDto { Name = "Dev" };

            _categoryRepoMock
                .Setup(x => x.GetCategoryByName(dto.Name))
                .ReturnsAsync(new TaskCategory());

            _categoryRepoMock
                .Setup(x => x.GetCategory(1))
                .ReturnsAsync(new TaskCategory());

            var result = await _categoryService.UpdateAsync(1, dto);

            Assert.False(result.Succeeded);
            Assert.Contains("Category with same name already exists.", result.Errors);
        }


        [Fact]
        public async Task UpdateAsync_Should_Return_Success_When_Update_Succeeds()
        {
            var dto = new TaskCategoryDto { Name = "Dev" };

            _categoryRepoMock
                .Setup(x => x.GetCategoryByName(dto.Name))
                .ReturnsAsync((TaskCategory?)null);

            _categoryRepoMock
                .Setup(x => x.GetCategory(1))
                .ReturnsAsync(new TaskCategory { TaskCategoryId = 1 });

            _categoryRepoMock
                .Setup(x => x.UpdateCategory(
                    It.IsAny<TaskCategory>(), dto))
                .ReturnsAsync(true);

            var result = await _categoryService.UpdateAsync(1, dto);

            Assert.True(result.Succeeded);
        }


        [Fact]
        public async Task UpdateAsync_Should_Return_Error_When_Update_Fails()
        {
            var dto = new TaskCategoryDto { Name = "Dev" };
            var category = new TaskCategory();

            _categoryRepoMock
                .Setup(x => x.GetCategoryByName(dto.Name))
                .ReturnsAsync((TaskCategory?)null);

            _categoryRepoMock
                .Setup(x => x.GetCategory(1))
                .ReturnsAsync(new TaskCategory());

            _categoryRepoMock
                .Setup(x => x.UpdateCategory(category, dto))
                .ReturnsAsync(false);

            var result = await _categoryService.UpdateAsync(1, dto);

            Assert.False(result.Succeeded);
            Assert.Contains("An unknown Error occured.", result.Errors);
        }
    }
}