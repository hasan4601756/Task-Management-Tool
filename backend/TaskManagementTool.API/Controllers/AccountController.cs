using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementTool.Application;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize]
        [HttpGet("roles")]
        public IActionResult GetUserRole()
        {
            var roles = User.FindAll(ClaimTypes.Role)
                            .Select(r => r.Value)
                            .ToList();

            return Ok(roles);
        }

        [HttpPost("register")]
        [Authorize(Roles="Admin")]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            var result = await _accountService.RegisterAsync(request);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var result = await _accountService.LoginAsync(dto);

            if (!result.Succeeded)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string RefreshToken)
        {
            var result = await _accountService.RefreshAsync(RefreshToken);

            if (!result.Succeeded)
                return Unauthorized();

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(string RefreshToken)
        {
            var result = await _accountService.LogoutAsync(RefreshToken);

            if (!result)
                return BadRequest("Invalid refresh token");

            return NoContent();
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _accountService.LogoutAllAsync(userId!);

            return NoContent();
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> UserProfile(){
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return NotFound();

            UserProfileDto? dto = await _accountService.GetUserProfileAsync(userId);

            return dto == null ? NotFound() : Ok(dto);
        }

        [Authorize]
        [HttpPut("profile/update")]
        public async Task<ActionResult<ResponseDto>> UpdateUserProfile(UserProfileDto dto){
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return BadRequest();

            return await _accountService.UpdateUserProfileAsync(userId, dto);
        }

        [Authorize]
        [HttpDelete("profile/{routeId?}")]
        public async Task<ActionResult<ResponseDto>> DeleteProfile(string? routeId)
        {
            var isAdmin = User.IsInRole("Admin");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _accountService.DeleteUserProfile(userId, routeId, isAdmin);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }
    }
}