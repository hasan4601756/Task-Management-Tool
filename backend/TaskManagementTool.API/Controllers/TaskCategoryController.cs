using System.Security.Claims;
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
        private ILogger<TaskCategoryController> _logger;

        public TaskCategoryController(ITaskCategoryService categoryRepo, ILogger<TaskCategoryController> logger)
        {
            _categoryRepo = categoryRepo;
            _logger = logger;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<TaskCategoryDto>>> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("GetAll requested by Admin {AdminId}", userId);

            var categories = await _categoryRepo.GetAllAsync();

            _logger.LogInformation("GetAll completed for Admin {AdminId}", userId);

            return Ok(categories);
        }
        
        [Authorize(Roles="Admin")]
        [HttpPost("add")]
        public async Task<ActionResult> Create([FromBody] TaskCategoryCreationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("Create requested by Admin {AdminId}", userId);

            var result = await _categoryRepo.AddAsync(dto);

            if (result.Succeeded)
            {
                _logger.LogInformation("Create completed for Admin {AdminId}", userId);
                return Created();
            }
            else
            {
                _logger.LogError("Create failed for Admin {AdminId} | Errors {@errors}", userId, result.Errors);
                return Conflict();
            }
        }

        [Authorize(Roles="Admin")]
        [HttpPut("update/{categoryId:int}")] 
        public async Task<ActionResult> Update([FromBody] TaskCategoryDto dto, int categoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("Update requested for Task Category {taskCategoryId} by Admin {AdminId}", dto.TaskCategoryId, userId);

            var result = await _categoryRepo.UpdateAsync(categoryId, dto);

            if (result.Succeeded)
            {
                _logger.LogInformation("Update completed for Task Category {taskCategoryId} by Admin {AdminId}", dto.TaskCategoryId, userId);
                return Ok();
            }
            else
            {
                _logger.LogError("Update failed for Task Category {taskCategoryId} by Admin {AdminId} | Errors {@errors}", dto.TaskCategoryId, userId, result.Errors);
                return BadRequest(new {error="Update failed"});
            }
        }

        [Authorize(Roles="Admin")]
        [HttpDelete("delete/{categoryId:int}")]
        public async Task<ActionResult> Delete(int categoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("Delete requested for Task Category {taskCategoryId} by Admin {AdminId}", categoryId, userId);

            var result = await _categoryRepo.RemoveAsync(categoryId);

            if (result.Succeeded)
            {
                _logger.LogInformation("Delete completed for Task Category {taskCategoryId} by Admin {AdminId}", categoryId, userId);
                return Ok();
            }
            else
            {
                _logger.LogError("Delete failed for Task Category {taskCategoryId} by Admin {AdminId} | Errors {@errors}", categoryId, userId, result.Errors);
                return BadRequest(new {error="Update failed"});
            }
        }
    }
}