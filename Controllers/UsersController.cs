using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst("id")?.Value ?? "0");
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse<TaskStatisticsDto>>> GetUserStatistics()
        {
            var userId = GetCurrentUserId();
            var result = await _userService.GetUserStatisticsAsync(userId);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] CreateUserDto updateUserDto)
        {
            var currentUserId = GetCurrentUserId();

            // Solo permitir actualizar su propio perfil
            if (currentUserId != id)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Datos inválidos", errors));
            }

            var result = await _userService.UpdateUserAsync(id, updateUserDto);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
        {
            var currentUserId = GetCurrentUserId();

            // Solo permitir eliminar su propio perfil
            if (currentUserId != id)
            {
                return Forbid();
            }

            var result = await _userService.DeleteUserAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}