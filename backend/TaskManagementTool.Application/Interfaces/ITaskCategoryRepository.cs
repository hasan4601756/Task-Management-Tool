using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Domain.Entities;

namespace TaskManagementTool.Application.Interfaces
{
    public interface ITaskCategoryRepository
    {
        Task<bool> CreateCategory(TaskCategory category);
        Task<bool> UpdateCategory(TaskCategory category, TaskCategoryDto dto);
        Task<IEnumerable<TaskCategory>> GetCategories();
        Task<TaskCategory?> GetCategory(int categoryId);
        Task<bool> DeleteCategory(TaskCategory category);
        Task<TaskCategory?> GetCategoryByName(string name);
    }
}