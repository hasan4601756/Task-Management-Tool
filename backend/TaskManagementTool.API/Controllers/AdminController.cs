using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("GetAllUsers requested by Admin {AdminId}", adminId);

            var users = await _adminService.GetAllUsers();

            _logger.LogInformation("GetAllUsers successfully completed for Admin {AdminId}", adminId);

            return Ok(users);
        }

        [HttpGet("tasks")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation("GetAllTasks requested by Admin {AdminId}", adminId);

            var tasks = await _adminService.GetAllTasks();

            _logger.LogInformation("GetAllTasks successully completed for Admin {AdminId}", adminId);

            return Ok(tasks);
        }

        [HttpPut("assigntask/{taskId:int}/{userId}")]
        public async Task<ActionResult<ResponseDto>> AssignTask(string userId, int taskId)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "AssignTask requested for assigning Task {TaskId} to User {UserId} by Admin {AdminId}", 
                taskId, userId, adminId
            );

            var response = await _adminService.AssignTask(userId, taskId);

            if (!response.Succeeded)
            {
                _logger.LogWarning(
                    "AssignTask failed for Task {TaskId} to User {UserId} by Admin {AdminId} | Errors: {@Errors}", 
                    taskId, userId, adminId, response.Errors
                );

                return BadRequest(new {Succeeded=false});
            }

            _logger.LogInformation(
                "AssignTask completed successfully for Task {TaskId} to User {UserId} by Admin {AdminId}", 
                taskId, userId, adminId
            );

            return Ok(new {Succeeded=true});
        }
    }
}