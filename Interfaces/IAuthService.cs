using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> RegisterAsync(CreateUserDto createUserDto);
        Task<ApiResponse<string>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId);
        string GenerateJwtToken(User user);
    }
}