using Microsoft.Extensions.Logging;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Domain.Entities;
using TaskManagementTool.Domain.Enums;

namespace TaskManagementTool.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepo;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepo, ILogger<TaskService> logger)
        {
            _taskRepo = taskRepo;
            _logger = logger;
        }
        public async Task<ResponseDto> AddAsync(TaskCreationDto dto, string userId)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                CreationDate = DateTime.UtcNow,
                AssignedUserId = userId,
                TaskCategoryId = dto.CategoryId,
                TaskStatus = TaskItemStatus.Pending,
                Priority = dto.Priority
            };

            var success = await _taskRepo.AddTaskAsync(task);

            return success ? new ResponseDto(){Succeeded = true} : new ResponseDto()
            {
                Succeeded = false,
                Errors = new String[]{"Something Unexpected happened"}
            };
        }

        public async Task<IEnumerable<TaskDto>> GetAllAsync(string userId)
        {
            try
            {
                var tasks = await _taskRepo.GetTasksByUserAsync(userId);

                var dtos = new List<TaskDto>();

                foreach (var task in tasks)
                {
                    dtos.Add(new TaskDto
                    {
                        Id = task.TaskItemId,
                        Title = task.Title,
                        TaskStatus = task.TaskStatus,
                        Priority = task.Priority
                    });
                }

                return dtos;
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Unknown exception caught at GetAllAsync");
                throw;
            }
        }

        public async Task<TaskDetailDto?> GetAsync(int taskId, string userId)
        {
            try
            {
                var task = await _taskRepo.GetTaskById(taskId);

                if (task == null) return null;
                else if(task.AssignedUserId != userId) return null;

                return new TaskDetailDto
                {
                    Id = task.TaskItemId,
                        Title = task.Title,
                        Description = task.Description,
                        CreationDate = task.CreationDate,
                        DueDate = task.DueDate,
                        TaskStatus = task.TaskStatus,
                        CategoryId = task.TaskCategoryId,
                        CategoryName = task.Category?.Name,
                        CategoryDescription = task.Category?.Description,
                        Priority = task.Priority
                }; 
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Unknown exception caught at GetAllAsync");
                throw;
            }
        }

        public async Task<ResponseDto> RemoveAsync(int taskId, string userId, bool isAdmin)
        {
            var task = await _taskRepo.GetTaskById(taskId);

            if (task == null)
                return new ResponseDto()
                {
                    Succeeded = false,
                    Errors = new String[]{"No user with the given id."}
                };

            if (task.AssignedUserId != userId && !isAdmin)
                return new ResponseDto(){
                    Succeeded = false,
                    Errors = new String[]{"Only task creator or Admin can remove the task."}
                };
            
            var success = await _taskRepo.DeleteTaskAsync(task);

            return success ? new ResponseDto(){Succeeded = true} : new ResponseDto()
            {
                Succeeded = false,
                Errors = new String[]{"Something Unexpected happened"}
            };
        }

        public async Task<ResponseDto> UpdateAsync(int taskId, TaskUpdationDto dto, string userId, bool isAdmin)
        {
            var task = await _taskRepo.GetTaskById(taskId);

            if (task == null)
                return new ResponseDto()
                {
                    Succeeded = false,
                    Errors = new String[]{"No task with the given id."}
                };

            if (task.AssignedUserId != userId && !isAdmin)
                return new ResponseDto(){
                    Succeeded = false,
                    Errors = new String[]{"Only task creator or Admin can remove the task."}
                };

            task.Title = dto.Title;
            task.Description = dto.Description; 
            task.DueDate = dto.DueDate;
            task.TaskStatus = dto.Status;
            task.TaskCategoryId = dto.CategoryId;
            task.Priority = dto.Priority;

            var success = await _taskRepo.UpdateTaskAsync(task);

            return success ? new ResponseDto(){Succeeded = true} : new ResponseDto()
            { 
                Succeeded = false,
                Errors = new String[]{"Something Unexpected happened"}
            };
        }

        public async Task<DashboardDto> GetDashboardAsync(string userId, bool isAdmin=false)
        {
            IEnumerable<TaskItem>? tasks = null;
            if (isAdmin){
                tasks = await _taskRepo.GetAllTasks();
            }
            else
            {
                tasks = await _taskRepo.GetTasksByUserAsync(userId);
            }

            var counts = tasks
                .GroupBy(t => t.TaskStatus)
                .ToDictionary(g => g.Key, g => g.Count());

            return new DashboardDto
            {
                CompletedTasks = counts.GetValueOrDefault(TaskItemStatus.Completed),
                InProgressTasks = counts.GetValueOrDefault(TaskItemStatus.InProgress),
                PendingTasks = counts.GetValueOrDefault(TaskItemStatus.Pending),
            };
        }
    }
}