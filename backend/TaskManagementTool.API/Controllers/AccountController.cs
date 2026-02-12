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
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("roles")]
        public IActionResult GetUserRole()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "GetUserRole request started for UserId {UserId}",
                userId);

            var roles = User.FindAll(ClaimTypes.Role)
                            .Select(r => r.Value)
                            .ToList();

            _logger.LogInformation(
                "GetUserRole completed for UserId {UserId} | Roles: {@Roles}",
                userId,
                roles);

            return Ok(roles);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "Register request initiated by AdminId {AdminId} for Username {Username}",
                adminId,
                request.UserName);

            var result = await _accountService.RegisterAsync(request);

            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "Register failed by AdminId {AdminId} for Username {Username} | Errors: {@Errors}",
                    adminId,
                    request.UserName,
                    result.Errors);

                return BadRequest(result.Errors);
            }

            _logger.LogInformation(
                "User registered successfully by AdminId {AdminId} for Username {Username}",
                adminId,
                request.UserName);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            _logger.LogInformation(
                "Login attempt for Email {Email}",
                dto.Email);

            var result = await _accountService.LoginAsync(dto);

            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "Login failed for Email {Email} | Errors: {@Errors}",
                    dto.Email,
                    result.Errors);

                return Unauthorized();
            }

            _logger.LogInformation(
                "Login successful for Email {Email}",
                dto.Email);

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            string refreshToken = Request.Cookies["refreshToken"];

            if (String.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("Token refresh failed | Errors: Empty refresh token");
                return Unauthorized(new {error= "Unauthorized"});
            }

            var result = await _accountService.RefreshAsync(refreshToken);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Token refresh failed | Errors: {@Errors}", 
                result.Errors);

                return Unauthorized(new {error = "invalid_refresh_token"});
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string refreshToken = Request.Cookies["refreshToken"];

            if (String.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("Logout failed | Errors: Empty refresh token");
                return Unauthorized(new {error= "logout failed"});
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "Logout requested for UserId {UserId}",
                userId);

            var result = await _accountService.LogoutAsync(refreshToken);

            if (!result)
            {
                _logger.LogWarning(
                    "Logout failed for UserId {UserId} | Errors: Invalid refresh token",
                    userId);

                return BadRequest();
            }

            _logger.LogInformation(
                "Logout successful for UserId {UserId}",
                userId);

            return NoContent();
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "LogoutAll requested for UserId {UserId}",
                userId);

            await _accountService.LogoutAllAsync(userId!);

            _logger.LogInformation(
                "LogoutAll completed for UserId {UserId}",
                userId);

            return NoContent();
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> UserProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "GetUserProfile requested for UserId {UserId}",
                userId);

            if (userId == null)
            {
                _logger.LogWarning("GetUserProfile failed | Errors: UserId not found in claims");
                return NotFound();
            }

            var dto = await _accountService.GetUserProfileAsync(userId);

            if (dto == null)
            {
                _logger.LogWarning(
                    "GetUserProfile not found for UserId {UserId}",
                    userId);

                return NotFound();
            }

            _logger.LogInformation(
                "GetUserProfile successful for UserId {UserId}",
                userId);

            return Ok(dto);
        }

        [Authorize]
        [HttpPut("profile/update")]
        public async Task<ActionResult<ResponseDto>> UpdateUserProfile([FromBody] UserProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation(
                "UpdateUserProfile requested for UserId {UserId}",
                userId);

            if (userId == null)
            {
                _logger.LogWarning("UpdateUserProfile failed | Errors: UserId not found");
                return BadRequest();
            }

            var response = await _accountService.UpdateUserProfileAsync(userId, dto);

            if (!response.Succeeded)
            {
                _logger.LogError("UpdateUserProfile failed for UserId {UserId} | Errors: {@erros}", userId, response.Errors);
                return BadRequest();
            }

            _logger.LogInformation(
                "UpdateUserProfile completed for UserId {UserId} | Success: {Success}",
                userId,
                response.Succeeded);

            return response;
        }

        [Authorize]
        [HttpDelete("profile/{routeId?}")]
        public async Task<ActionResult<ResponseDto>> DeleteProfile(string? routeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            _logger.LogInformation(
                "DeleteProfile requested by UserId {UserId} | TargetUserId {TargetUserId} | IsAdmin {IsAdmin}",
                userId,
                routeId,
                isAdmin);

            var result = await _accountService.DeleteUserProfile(userId, routeId, isAdmin);

            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "DeleteProfile failed by UserId {UserId} | TargetUserId {TargetUserId} | Errors {@errors}",
                    userId,
                    routeId,
                    result.Errors);

                return BadRequest();
            }

            _logger.LogInformation(
                "DeleteProfile successful by UserId {UserId} | TargetUserId {TargetUserId}",
                userId,
                routeId);

            return Ok(result);
        }
    }
}
