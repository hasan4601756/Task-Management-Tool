using TaskManagementTool.Application.DTOs;

namespace TaskManagementTool.Application.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<IEnumerable<TaskDto>> GetAllTasks();
        Task<ResponseDto> AssignTask(string userId, int taskId);
    }
}