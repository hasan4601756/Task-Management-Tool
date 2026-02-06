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

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _adminService.GetAllUsers();

            return Ok(users);
        }

        [HttpGet("tasks")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
        {
            var tasks = await _adminService.GetAllTasks();

            return Ok(tasks);
        }

        [HttpPut("assigntask/{taskId:int}/{userId}")]
        public async Task<ActionResult<ResponseDto>> AssignTask(string userId, int taskId)
        {
            var response = await _adminService.AssignTask(userId, taskId);

            return response;
        }
    }
}