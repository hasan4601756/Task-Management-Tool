using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Application.Services;
using TaskManagementTool.Domain.Entities;
using TaskManagementTool.Domain.Enums;

namespace TaskManagementTool.Tests.Services
{
    public class TaskServiceTests
    {
        private readonly TaskService _taskService;
        private readonly Mock<ITaskRepository> _taskRepoMock;
        private readonly Mock<ILogger<TaskService>> _loggerMock;

        public TaskServiceTests()
        {
            _taskRepoMock = new Mock<ITaskRepository>();
            _loggerMock = new Mock<ILogger<TaskService>>();

            _taskService = new TaskService(_taskRepoMock.Object, _loggerMock.Object); 
        }

        [Fact]
        public async Task AddAsync_Should_Return_Success_When_Add_Succeeds()
        {
            var dto = new TaskCreationDto
            {
                Title = "Task 1",
                Description = "Desc",
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                CategoryId = 3
            };

            _taskRepoMock
                .Setup(x => x.AddTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(true);

            var result = await _taskService.AddAsync(dto, "user1");

            Assert.True(result.Succeeded);

            _taskRepoMock.Verify(x => x.AddTaskAsync(
                It.Is<TaskItem>(t =>
                    t.Title == dto.Title &&
                    t.AssignedUserId == "user1" &&
                    t.TaskStatus == TaskItemStatus.Pending)),
                Times.Once);
        }

        [Fact]
        public async Task AddAsync_Should_Return_Error_When_Add_Fails()
        {
            var dto = new TaskCreationDto();

            _taskRepoMock
                .Setup(x => x.AddTaskAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(false);

            var result = await _taskService.AddAsync(dto, "user1");

            Assert.False(result.Succeeded);
            Assert.Contains("Something Unexpected happened", result.Errors);
        }

        [Fact]
        public async Task GetAllAsync_Should_Log_And_Throw_When_Exception_Occurs()
        {
            var ex = new Exception("DB Error");

            _taskRepoMock
                .Setup(x => x.GetTasksByUserAsync("user1"))
                .ThrowsAsync(ex);

            await Assert.ThrowsAsync<Exception>(() => _taskService.GetAllAsync("user1"));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString().Contains("Unknown exception caught at GetAllAsync")),
                    ex,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }


        [Fact]
        public async Task GetAsync_Should_Return_Null_When_Task_Not_Found()
        {
            _taskRepoMock
                .Setup(x => x.GetTaskById(1))
                .ReturnsAsync((TaskItem?)null);

            var result = await _taskService.GetAsync(1, "user1");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_Should_Return_Null_When_User_Not_Owner()
        {
            _taskRepoMock
                .Setup(x => x.GetTaskById(1))
                .ReturnsAsync(new TaskItem { AssignedUserId = "otherUser" });

            var result = await _taskService.GetAsync(1, "user1");

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_Should_Return_TaskDetail_When_Valid()
        {
            var task = new TaskItem
            {
                TaskItemId = 1,
                Title = "T1",
                Description = "Desc",
                AssignedUserId = "user1",
                TaskStatus = TaskItemStatus.Pending,
                TaskCategoryId = 2,
                Category = new TaskCategory
                {
                    Name = "Dev",
                    Description = "Development"
                }
            };

            _taskRepoMock
                .Setup(x => x.GetTaskById(1))
                .ReturnsAsync(task);

            var result = await _taskService.GetAsync(1, "user1");

            Assert.NotNull(result);
            Assert.Equal("T1", result.Title);
            Assert.Equal("Dev", result.CategoryName);
        }

        [Fact]
        public async Task RemoveAsync_Should_Return_Error_When_Not_Found()
        {
            _taskRepoMock
                .Setup(x => x.GetTaskById(1))
                .ReturnsAsync((TaskItem?)null);

            var result = await _taskService.RemoveAsync(1, "user1", false);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task RemoveAsync_Should_Return_Error_When_Not_Authorized()
        {
            _taskRepoMock
                .Setup(x => x.GetTaskById(1))
                .ReturnsAsync(new TaskItem { AssignedUserId = "otherUser" });

            var result = await _taskService.RemoveAsync(1, "user1", false);

            Assert.False(result.Succeeded);
            Assert.Contains("Only task creator or Admin can remove the task.", result.Errors);
        }

        [Fact]
        public async Task RemoveAsync_Should_Allow_Admin()
        {
            var task = new TaskItem { AssignedUserId = "otherUser" };

            _taskRepoMock
                .Setup(x => x.GetTaskById(1))
                .ReturnsAsync(task);

            _taskRepoMock
                .Setup(x => x.DeleteTaskAsync(task))
                .ReturnsAsync(true);

            var result = await _taskService.RemoveAsync(1, "user1", true);

            Assert.True(result.Succeeded);
        }


        [Fact]
        public async Task UpdateAsync_Should_Return_Error_When_Not_Found()
        {
            _taskRepoMock
                .Setup(x => x.GetTaskById(1))
                .ReturnsAsync((TaskItem?)null);

            var result = await _taskService.UpdateAsync(1, new TaskUpdationDto(), "user1", false);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task UpdateAsync_Should_Return_Error_When_Not_Authorized()
        {
            _taskRepoMock
                .Setup(x => x.GetTaskById(1))
                .ReturnsAsync(new TaskItem { AssignedUserId = "otherUser" });

            var result = await _taskService.UpdateAsync(1, new TaskUpdationDto(), "user1", false);

            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_When_Owner()
        {
            var task = new TaskItem { AssignedUserId = "user1" };

            var dto = new TaskUpdationDto
            {
                Title = "Updated",
                Description = "Updated Desc",
                Status = TaskItemStatus.Completed,
                CategoryId = 5
            };

            _taskRepoMock
                .Setup(x => x.GetTaskById(1))
                .ReturnsAsync(task);

            _taskRepoMock
                .Setup(x => x.UpdateTaskAsync(task))
                .ReturnsAsync(true);

            var result = await _taskService.UpdateAsync(1, dto, "user1", false);

            Assert.True(result.Succeeded);
            Assert.Equal("Updated", task.Title);
        }

        [Fact]
        public async Task GetDashboardAsync_Should_Return_Correct_Counts()
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem { TaskStatus = TaskItemStatus.Completed },
                new TaskItem { TaskStatus = TaskItemStatus.Completed },
                new TaskItem { TaskStatus = TaskItemStatus.Pending },
                new TaskItem { TaskStatus = TaskItemStatus.InProgress }
            };

            _taskRepoMock
                .Setup(x => x.GetTasksByUserAsync("user1"))
                .ReturnsAsync(tasks);

            var result = await _taskService.GetDashboardAsync("user1");

            Assert.Equal(2, result.CompletedTasks);
            Assert.Equal(1, result.PendingTasks);
            Assert.Equal(1, result.InProgressTasks);
        }
    }
}