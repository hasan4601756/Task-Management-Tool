using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class TaskCategoryController : ControllerBase
    {
        private readonly ITaskCategoryService _categoryRepo;

        public TaskCategoryController(ITaskCategoryService categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<TaskCategoryDto>>> GetAll()
        {
            var categories = await _categoryRepo.GetAllAsync();

            return Ok(categories);
        }
        
        [Authorize(Roles="Admin")]
        [HttpPost("add")]
        public async Task<ActionResult> Create(TaskCategoryCreationDto dto)
        {
            var result = await _categoryRepo.AddAsync(dto);

            return result.Succeeded? Created() : Conflict(result.Errors);
        }

        [Authorize(Roles="Admin")]
        [HttpPut("update/{categoryId:int}")] 
        public async Task<ActionResult> Update(TaskCategoryDto dto, int categoryId)
        {
            var result = await _categoryRepo.UpdateAsync(categoryId, dto);

            return result.Succeeded? Ok() : BadRequest(result.Errors);
        }

        [Authorize(Roles="Admin")]
        [HttpDelete("delete/{categoryId:int}")]
        public async Task<ActionResult> Delete(int categoryId)
        {
            var result = await _categoryRepo.RemoveAsync(categoryId);

            return result.Succeeded? Ok() : BadRequest(result.Errors);
        }
    }
}