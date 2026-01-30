using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Infrastructure.Data;
using TaskManagementTool.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementTool.Infrastructure.Repository{
    public class TaskRepository : ITaskRepository{
        private readonly AppDbContext _dbContext;

        public TaskRepository(AppDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<bool> AddTaskAsync(TaskItem task)
        {
            await _dbContext.Tasks.AddAsync(task);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateTaskAsync(TaskItem task)
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksByUserAsync(string userId)
        {
            var tasks = await _dbContext.Tasks
                .Include(t => t.Category)
                .Where(t => t.AssignedUserId == userId)
                .ToListAsync();

            return tasks;
        }

        public async Task<TaskItem?> GetTaskById(int taskId)
        {
            var task = await _dbContext.Tasks.FindAsync(taskId);

            if (task == null) return null;  

            return task;
        }

        public async Task<bool> DeleteTaskAsync(TaskItem task)
        {
            _dbContext.Tasks.Remove(task);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}