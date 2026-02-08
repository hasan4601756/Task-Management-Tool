using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.API.Controllers{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Authorize]
        [HttpGet("dashboard")]
        public async Task<ActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return NotFound();

            var dashboard = await _taskService.GetDashboardAsync(userId);

            return Ok(dashboard);
        }

        [HttpGet("tasks")]
        public async Task<ActionResult> GetAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tasks = await _taskService.GetAllAsync(userId);
            return Ok(tasks);
        }

        [HttpGet("tasks/{taskId:int}")]
        public async Task<ActionResult> GetDetail(int taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tasks = await _taskService.GetAsync(taskId, userId);
            return Ok(tasks);
        }

        [HttpPost("add")]
        public async Task<ActionResult> Create(TaskCreationDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var response = await _taskService.AddAsync(dto, userId);
            return response.Succeeded ? Ok() : BadRequest();
        }

        [HttpPut("update/{taskId:int}")]
        public async Task<ActionResult> Update(TaskUpdationDto dto, int taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var response = await _taskService.UpdateAsync(taskId, dto, userId, isAdmin);
            return response.Succeeded ? Ok(response) : NotFound(response);
        }

        [HttpDelete("delete/{taskId:int}")]
        public async Task<ActionResult> Delete(int taskId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var response = await _taskService.RemoveAsync(taskId, userId, isAdmin);
            return response.Succeeded ? Ok() : NotFound();
        }
    }
}