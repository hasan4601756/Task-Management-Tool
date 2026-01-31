using System.Security.Principal;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Domain.Entities;

namespace TaskManagementTool.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IIdentityRepository _identityRepo;
        private readonly ITaskRepository _taskRepo;

        public AdminService(IIdentityRepository identityRepo, ITaskRepository taskRepo)
        {
            _identityRepo = identityRepo;
            _taskRepo = taskRepo;
        }
        public async Task<ResponseDto> AssignTask(string userId, int taskId)
        {
            var task = await _taskRepo.GetTaskById(taskId);

            if (task == null) 
            return new ResponseDto()
                {
                    Succeeded = false,
                    Errors = new String[]{"No task present with the given Id."}
                };
            
            task.AssignedUserId = userId;

            var success = await _taskRepo.UpdateTaskAsync(task);

            return success ? new ResponseDto(){Succeeded = true} : new ResponseDto()
            {
                Succeeded = false,
                Errors = new String[]{"Something Unexpected happened"}
            };
        }

        public async Task<IEnumerable<TaskDto>> GetAllTasks()
        {
           var tasks = await _taskRepo.GetAllTasks();
           var taskdto = new List<TaskDto>();

           foreach (var task in tasks)
            {
                taskdto.Add(new TaskDto
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

            return taskdto;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await _identityRepo.GetAllUsers();
            var userdtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userdto = new UserDto()
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email
                };

                userdtos.Add(userdto);
            }

            return userdtos;
        }
    }
}