using Microsoft.Extensions.Logging;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IIdentityRepository _identityRepo;
        private readonly ITaskRepository _taskRepo;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IIdentityRepository identityRepo, ITaskRepository taskRepo, ILogger<AdminService> logger)
        {
            _identityRepo = identityRepo;
            _taskRepo = taskRepo;
            _logger = logger;
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
            
            if (userId == null) 
            return new ResponseDto()
                {
                    Succeeded = false,
                    Errors = new String[]{"Cannot assign null Id."}
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
            try
            {
                var tasks = await _taskRepo.GetAllTasks();
                var taskdto = new List<TaskDto>();

                foreach (var task in tasks)
                {
                    taskdto.Add(new TaskDto
                    {
                        Id = task.TaskItemId,
                        Title = task.Title,
                        TaskStatus = task.TaskStatus,
                        UserName = (await _identityRepo.FindByIdAsync(task.AssignedUserId))?.UserName
                    });
                }

                return taskdto;
            } catch(Exception ex)
            {
                _logger.LogError(ex,"Task fetch for Admin failed");
                throw;
            }
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            try
            {
                var users = await _identityRepo.GetAllUsers();
                // var userdtos = new List<UserDto>();

                // foreach (var user in users)
                // {
                //     var userdto = new UserDto()
                //     {
                //         UserId = user.UserId,
                //         UserName = user.UserName,
                //         Email = user.Email
                //     };

                //     userdtos.Add(userdto);
                // }

                return users;
            } catch(Exception ex)
            {
                _logger.LogError(ex, "User fetch for Admin failed");
                throw;
            }
        }

        public async Task<UserDto?> GetTaskUser(int taskId)
        {
            try
            {
                    var task = await _taskRepo.GetTaskById(taskId);

                if (task == null) return null;
                var user = await _identityRepo.FindByIdAsync(task.AssignedUserId);

                if (user == null) return null;
                var userdto = new UserDto
                {
                    UserId = task.AssignedUserId,
                    UserName = user.UserName,
                    Email = user.Email
                };

                return userdto;
            } catch(Exception ex)
            {
                _logger.LogError(ex, "User fetch for Admin by taskId failed");
                throw;
            }
        }
    }
}