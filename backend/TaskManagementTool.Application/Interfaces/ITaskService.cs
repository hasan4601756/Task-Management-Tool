using TaskManagementTool.Application.DTOs;

namespace TaskManagementTool.Application.Interfaces{
    public interface ITaskService
    {
        Task<ResponseDto> AddAsync(TaskCreationDto dto, string userId);
        Task<ResponseDto> UpdateAsync(int taskId, TaskUpdationDto dto, string userId);
        Task<IEnumerable<TaskDto>> GetAllAsync(string userId);
        Task<TaskDto?> GetAsync(int taskId);
        Task<ResponseDto> RemoveAsync(int taskId, string userId);
    }
}