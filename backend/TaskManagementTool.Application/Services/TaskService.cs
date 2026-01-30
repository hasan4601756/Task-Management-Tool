using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Domain.Entities;

namespace TaskManagementTool.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepo;

        public TaskService(ITaskRepository taskRepo)
        {
            _taskRepo = taskRepo;
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
                TaskCategoryId = dto.CategoryId
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
            var tasks = await _taskRepo.GetTasksByUserAsync(userId);

            var dtos = new List<TaskDto>();

            foreach (var task in tasks)
            {
                dtos.Add(new TaskDto
                {
                    Id = task.TaskItemId,
                    Title = task.Title,
                    Description = task.Description,
                    CreationDate = task.CreationDate,
                    DueDate = task.DueDate,
                    TaskStatus = task.TaskStatus,
                    CategoryName = task.Category?.Name,
                    CategoryDescription = task.Category?.Description
                });
            }

            return dtos;
        }

        public async Task<TaskDto?> GetAsync(int taskId)
        {
            var task = await _taskRepo.GetTaskById(taskId);

            return new TaskDto
            {
                Id = task.TaskItemId,
                    Title = task.Title,
                    Description = task.Description,
                    CreationDate = task.CreationDate,
                    DueDate = task.DueDate,
                    TaskStatus = task.TaskStatus,
                    CategoryName = task.Category?.Name,
                    CategoryDescription = task.Category?.Description
            }; 
        }

        public async Task<ResponseDto> RemoveAsync(int taskId, string userId)
        {
            var task = await _taskRepo.GetTaskById(taskId);

            if (task == null)
                return new ResponseDto()
                {
                    Succeeded = false,
                    Errors = new String[]{"No user with the given id."}
                };

            if (task.AssignedUserId != userId)
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

        public async Task<ResponseDto> UpdateAsync(int taskId, TaskUpdationDto dto, string userId)
        {
            var task = await _taskRepo.GetTaskById(taskId);

            if (task == null)
                return new ResponseDto()
                {
                    Succeeded = false,
                    Errors = new String[]{"No user with the given id."}
                };

            if (task.AssignedUserId != userId)
                return new ResponseDto(){
                    Succeeded = false,
                    Errors = new String[]{"Only task creator or Admin can remove the task."}
                };

            task.Title = dto.Title;
            task.Description = dto.Description; 
            task.DueDate = dto.DueDate;
            task.TaskStatus = dto.Status;
            task.TaskCategoryId = dto.CategoryId;

            var success = await _taskRepo.UpdateTaskAsync(task);

            return success ? new ResponseDto(){Succeeded = true} : new ResponseDto()
            {
                Succeeded = false,
                Errors = new String[]{"Something Unexpected happened"}
            };
        }
    }
}