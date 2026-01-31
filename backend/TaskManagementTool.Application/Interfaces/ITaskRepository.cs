using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Domain.Entities;

namespace TaskManagementTool.Application.Interfaces{
    public interface ITaskRepository{
        Task<bool> AddTaskAsync(TaskItem task);
        Task<bool> UpdateTaskAsync(TaskItem task);
        Task<IEnumerable<TaskItem>> GetTasksByUserAsync(string userId);
        Task<TaskItem?> GetTaskById(int taskId);
        Task<bool> DeleteTaskAsync(TaskItem task);
        Task<IEnumerable<TaskItem>> GetAllTasks();
    }
}