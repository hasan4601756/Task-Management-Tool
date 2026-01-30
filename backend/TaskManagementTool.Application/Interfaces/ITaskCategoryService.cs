using TaskManagementTool.Application.DTOs;

namespace TaskManagementTool.Application.Interfaces{
    public interface ITaskCategoryService
    {
        Task<ResponseDto> AddAsync(TaskCategoryCreationDto dto);
        Task<TaskCategoryDto?> GetAsync(int categoryId);
        Task<IEnumerable<TaskCategoryDto>> GetAllAsync();
        Task<ResponseDto> RemoveAsync(int categoryId);
        Task<ResponseDto> UpdateAsync(int categoryId, TaskCategoryDto dto);
    }
}