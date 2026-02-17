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
        private readonly ILogger<TaskController> _logger;

        public TaskController(
            ITaskService taskService,
            ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("UserId claim missing");
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult> Dashboard()
        {
            var userId = GetUserId();
            
            var isAdmin = User.IsInRole("Admin");

            _logger.LogInformation(
                "Dashboard requested by User {UserId}",
                userId);

            var dashboard = await _taskService.GetDashboardAsync(userId, isAdmin);

            _logger.LogInformation(
                "Dashboard completed for User {UserId}",
                userId);

            return Ok(dashboard);
        }

        [HttpGet("tasks")]
        public async Task<ActionResult> GetAll()
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "GetAll requested by User {UserId}",
                userId);

            var tasks = await _taskService.GetAllAsync(userId);

            _logger.LogInformation(
                "GetAll completed for User {UserId}",
                userId);

            return Ok(tasks);
        }

        [HttpGet("tasks/{taskId:int}")]
        public async Task<ActionResult> GetDetail(int taskId)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "GetDetail requested for Task {TaskId} by User {UserId}",
                taskId,
                userId);

            var task = await _taskService.GetAsync(taskId, userId);

            if (task == null)
            {
                _logger.LogWarning(
                    "GetDetail failed: Task {TaskId} not found for User {UserId}",
                    taskId,
                    userId);

                return NotFound();
            }

            _logger.LogInformation(
                "GetDetail completed for Task {TaskId} by User {UserId}",
                taskId,
                userId);

            return Ok(task);
        }

        [HttpPost("add")]
        public async Task<ActionResult> Create([FromBody] TaskCreationDto dto)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Create requested by User {UserId}",
                userId);

            var response = await _taskService.AddAsync(dto, userId);

            if (!response.Succeeded)
            {
                _logger.LogWarning(
                    "Create failed for User {UserId} | Errors {@errors}",
                    userId, response.Errors);

                return BadRequest();
            }

            _logger.LogInformation(
                "Create completed for User {UserId}",
                userId);

            return Ok(response);
        }

        [HttpPut("update/{taskId:int}")]
        public async Task<ActionResult> Update(
            int taskId,
            [FromBody] TaskUpdationDto dto)
        {
            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin");

            _logger.LogInformation(
                "Update requested by {ActorType} {UserId} for Task {TaskId}",
                isAdmin ? "Admin" : "User",
                userId,
                taskId);

            var response = await _taskService.UpdateAsync(
                taskId,
                dto,
                userId,
                isAdmin);

            if (!response.Succeeded)
            {
                _logger.LogWarning(
                "Update failed by {ActorType} {UserId} for Task {TaskId} | Errors {@errors}",
                isAdmin ? "Admin" : "User",
                userId,
                taskId,
                response.Errors);

                return BadRequest(new {error="Update failed"});
            }

            _logger.LogInformation(
                "Update completed by {ActorType} {UserId} for Task {TaskId}",
                isAdmin ? "Admin" : "User",
                userId,
                taskId);

            return Ok();
        }

        [HttpDelete("delete/{taskId:int}")]
        public async Task<ActionResult> Delete(int taskId)
        {
            var userId = GetUserId();
            var isAdmin = User.IsInRole("Admin");

            _logger.LogInformation(
                "Delete requested by {ActorType} {UserId} for Task {TaskId}",
                isAdmin ? "Admin" : "User",
                userId,
                taskId);

            var response = await _taskService.RemoveAsync(
                taskId,
                userId,
                isAdmin);

            if (!response.Succeeded)
            {
                _logger.LogWarning(
                "Delete failed by {ActorType} {UserId} for Task {TaskId} | Errors {@errors}",
                isAdmin ? "Admin" : "User",
                userId,
                taskId,
                response.Errors);

                return BadRequest(new {error="Update failed"});
            }

            _logger.LogInformation(
                "Delete completed by {ActorType} {UserId} for Task {TaskId}",
                isAdmin ? "Admin" : "User",
                userId,
                taskId);

            return Ok();
        }
    }
}