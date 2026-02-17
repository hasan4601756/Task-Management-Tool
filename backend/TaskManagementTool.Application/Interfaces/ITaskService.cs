using TaskManagementTool.Application.DTOs;

namespace TaskManagementTool.Application.Interfaces{
    public interface ITaskService
    { 
        Task<ResponseDto> AddAsync(TaskCreationDto dto, string userId);
        Task<ResponseDto> UpdateAsync(int taskId, TaskUpdationDto dto, string userId, bool isAdmin);
        Task<IEnumerable<TaskDto>> GetAllAsync(string userId);
        Task<TaskDetailDto?> GetAsync(int taskId, string userId);
        Task<ResponseDto> RemoveAsync(int taskId, string userId, bool isAdmin);
        Task<DashboardDto> GetDashboardAsync(string userId, bool isAdmin=false);
    }
}