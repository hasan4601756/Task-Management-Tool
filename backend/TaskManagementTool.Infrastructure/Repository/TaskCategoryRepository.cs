using Microsoft.EntityFrameworkCore;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Domain.Entities;
using TaskManagementTool.Infrastructure.Data;

namespace TaskManagementTool.Infrastructure.Repository
{
    public class TaskCategoryRepository : ITaskCategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public TaskCategoryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> CreateCategory(TaskCategory category)
        {
            await _dbContext.TaskCategories.AddAsync(category);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategory(TaskCategory category)
        {
            _dbContext.TaskCategories.Remove(category);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<TaskCategory>> GetCategories()
        {
            return await _dbContext.TaskCategories
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TaskCategory?> GetCategory(int categoryId)
        {
            return await _dbContext.TaskCategories.FindAsync(categoryId);
        }

        public async Task<bool> UpdateCategory(TaskCategory category, TaskCategoryDto dto)
        {
            category.Name = dto.Name;
            category.Description = dto.Description;

            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<TaskCategory?> GetCategoryByName(string name)
        {
            return await _dbContext.TaskCategories.Where(c => c.Name == name).FirstOrDefaultAsync();
        }
    }
}