using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserDto>>> GetAllUsersAsync();
        Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);
        Task<ApiResponse<UserDto>> UpdateUserAsync(int id, CreateUserDto updateUserDto);
        Task<ApiResponse<bool>> DeleteUserAsync(int id);
        Task<ApiResponse<TaskStatisticsDto>> GetUserStatisticsAsync(int userId);
    }
}