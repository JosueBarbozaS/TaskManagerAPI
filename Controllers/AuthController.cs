using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<string>>> Register([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<string>.ErrorResponse("Datos inválidos", errors));
            }

            var result = await _authService.RegisterAsync(createUserDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<string>>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<string>.ErrorResponse("Datos inválidos", errors));
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
        {
            var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
            var result = await _authService.GetCurrentUserAsync(userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}