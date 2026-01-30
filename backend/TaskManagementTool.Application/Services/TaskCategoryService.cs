using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;
using TaskManagementTool.Domain.Entities;

namespace TaskManagementTool.Application.Services
{
    public class TaskCategoryService : ITaskCategoryService
    {
        private readonly ITaskCategoryRepository _categoryRepo;

        public TaskCategoryService(ITaskCategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        public async Task<ResponseDto> AddAsync(TaskCategoryCreationDto dto)
        {
            var nameExists = (await _categoryRepo.GetCategoryByName(dto.Name)) != null;

            if (nameExists) 
                return new ResponseDto
                {
                    Succeeded = false,
                    Errors = new String[] {"Category with same name already exists."}
                };

            TaskCategory category = new TaskCategory
            {
                Name = dto.Name,
                Description = dto.Description
            };

            var result = await _categoryRepo.CreateCategory(category);
            return result ? new ResponseDto(){Succeeded = true} : new ResponseDto
                {
                    Succeeded = false,
                    Errors = new String[] {"An unknown Error occurred."}
                };
        }

        public async Task<IEnumerable<TaskCategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepo.GetCategories();
            return categories.Select(c => new TaskCategoryDto
                {
                    TaskCategoryId = c.TaskCategoryId,
                    Name = c.Name,
                    Description = c.Description!
                });
        }

        public async Task<TaskCategoryDto?> GetAsync(int categoryId)
        {
            var category = await _categoryRepo.GetCategory(categoryId);

            if (category == null) return null;

            return new TaskCategoryDto
            {
                Name = category.Name,
                TaskCategoryId = category.TaskCategoryId,
                Description = category.Description
            };
        }

        public async Task<ResponseDto> RemoveAsync(int categoryId)
        {
            var category = await _categoryRepo.GetCategory(categoryId);
            if (category == null) return new ResponseDto
            {
                Succeeded = false,
                Errors = new String[]{"No category with given id exists."}
            };

            var result = await _categoryRepo.DeleteCategory(category);

            return result ? new ResponseDto(){Succeeded=true} : new ResponseDto(){
                Succeeded = true,
                Errors = new String[]{"An unknown Error occured"}
            };
        }

        public async Task<ResponseDto> UpdateAsync(int categoryId, TaskCategoryDto dto)
        {
            var category = await _categoryRepo.GetCategoryByName(dto.Name);

            if (category == null) 
                return new ResponseDto
                {
                    Succeeded = false,
                    Errors = new String[] {"Category with same name already exists."}
                };

            var result = await _categoryRepo.UpdateCategory(category, dto);

            return result ? new ResponseDto(){Succeeded=true} : new ResponseDto(){
                Succeeded = false,
                Errors = new String[]{"An unknown Error occured."}
            };
        }
    }
}