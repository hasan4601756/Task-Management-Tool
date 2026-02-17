using Microsoft.Extensions.Logging;
using Moq;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Application.Services;
using TaskManagementTool.Domain.Entities;
using TaskManagementTool.Domain.Enums;

namespace TaskManagementTool.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly IAdminService _adminService;
        private readonly Mock<ITaskRepository> _taskRepoMock;
        private readonly Mock<IIdentityRepository> _identityRepoMock;
        private readonly Mock<ILogger<AdminService>> _logger;

        public AdminServiceTests()
        {
            _taskRepoMock = new Mock<ITaskRepository>();
            _identityRepoMock = new Mock<IIdentityRepository>();
            _logger = new Mock<ILogger<AdminService>>();
            _adminService = new AdminService(_identityRepoMock.Object, _taskRepoMock.Object, _logger.Object);
        }

        [Fact]
        public async Task AssignTask_Should_Return_Error_When_Task_Not_Found()
        {
            var userId = "user1";
            var taskId = 10;

            _taskRepoMock
                .Setup(x => x.GetTaskById(taskId))
                .ReturnsAsync((TaskItem?)null);

            var result = await _adminService.AssignTask(userId, taskId);

            Assert.False(result.Succeeded);
            Assert.Contains("No task present with the given Id.", result.Errors);

            _taskRepoMock.Verify(x => x.UpdateTaskAsync(It.IsAny<TaskItem>()), Times.Never);
        }

        [Fact]
        public async Task AssignTask_Should_Return_Success_When_Update_Succeeds()
        {
            var userId = "user1";
            var userId_before = "user before";
            var taskId = 10;

            var task = new TaskItem
            {
                TaskItemId = taskId,
                AssignedUserId = userId_before
            };

            _taskRepoMock
                .Setup(x => x.GetTaskById(taskId))
                .ReturnsAsync(task);

            _taskRepoMock
                .Setup(x => x.UpdateTaskAsync(task))
                .ReturnsAsync(true);

            var result = await _adminService.AssignTask(userId, taskId);

            Assert.True(result.Succeeded);
            Assert.Null(result.Errors);
            Assert.Equal(userId, task.AssignedUserId);

            _taskRepoMock.Verify(x => x.UpdateTaskAsync(task), Times.Once);
        }

        [Fact]
        public async Task AssignTask_Should_Return_Error_When_Update_Fails()
        {
            var userId = "user1";
            var taskId = 10;

            var task = new TaskItem
            {
                TaskItemId = taskId
            };

            _taskRepoMock
                .Setup(x => x.GetTaskById(taskId))
                .ReturnsAsync(task);

            _taskRepoMock
                .Setup(x => x.UpdateTaskAsync(task))
                .ReturnsAsync(false);

            var result = await _adminService.AssignTask(userId, taskId);

            Assert.False(result.Succeeded);
            Assert.Contains("Something Unexpected happened", result.Errors);

            _taskRepoMock.Verify(x => x.UpdateTaskAsync(task), Times.Once);
        }

        [Fact]
        public async Task GetAllTasks_Should_Return_Mapped_TaskDtos()
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    TaskItemId = 1,
                    Title = "Task 1",
                    TaskStatus = TaskItemStatus.Pending
                },
                new TaskItem
                {
                    TaskItemId = 2,
                    Title = "Task 2",
                    TaskStatus = TaskItemStatus.Completed
                }
            };

            _taskRepoMock
                .Setup(x => x.GetAllTasks())
                .ReturnsAsync(tasks);

            var result = await _adminService.GetAllTasks();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Id == 1 && t.Title == "Task 1");
            Assert.Contains(result, t => t.Id == 2 && t.TaskStatus == TaskItemStatus.Completed);

            _taskRepoMock.Verify(x => x.GetAllTasks(), Times.Once);
        }

        [Fact]
        public async Task GetAllTasks_Should_Log_Error_And_Throw_When_Exception_Occurs()
        {
            var exception = new Exception("DB Failure");

            _taskRepoMock
                .Setup(x => x.GetAllTasks())
                .ThrowsAsync(exception);

            await Assert.ThrowsAsync<Exception>(() => _adminService.GetAllTasks());

            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString().Contains("Task fetch for Admin failed")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

        }

        [Fact]
        public async Task GetAllUsers_Should_Return_Mapped_UserDtos()
        {
            var users = new List<UserDto>
            {
                new UserDto
                {
                    UserId = "1",
                    UserName = "User1",
                    Email = "user1@test.com"
                },
                new UserDto
                {
                    UserId = "2",
                    UserName = "User2",
                    Email = "user2@test.com"
                }
            };

            _identityRepoMock
                .Setup(x => x.GetAllUsers())
                .ReturnsAsync(users);

            var result = await _adminService.GetAllUsers();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.UserName == "User1");
            Assert.Contains(result, u => u.Email == "user2@test.com");

            _identityRepoMock.Verify(x => x.GetAllUsers(), Times.Once);
        }

        [Fact]
        public async Task GetAllUsers_Should_Log_Error_And_Throw_When_Exception_Occurs()
        {
            var exception = new Exception("User Fetch Failure");

            _identityRepoMock
                .Setup(x => x.GetAllUsers())
                .ThrowsAsync(exception);

            await Assert.ThrowsAsync<Exception>(() => _adminService.GetAllUsers());

            _logger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) =>
                        v.ToString().Contains("User fetch for Admin failed")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

        }
    }
}