using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly TaskManagerDbContext _context;

        public UserService(TaskManagerDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.IsActive)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        CreatedAt = u.CreatedAt,
                        IsActive = u.IsActive
                    })
                    .ToListAsync();

                return ApiResponse<List<UserDto>>.SuccessResponse(users);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UserDto>>.ErrorResponse($"Error al obtener usuarios: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("Usuario no encontrado");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                };

                return ApiResponse<UserDto>.SuccessResponse(userDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error al obtener usuario: {ex.Message}");
            }
        }

        public async Task<ApiResponse<UserDto>> UpdateUserAsync(int id, CreateUserDto updateUserDto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("Usuario no encontrado");
                }

                // Verificar si el username o email ya existen en otros usuarios
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => (u.Username == updateUserDto.Username || u.Email == updateUserDto.Email) && u.Id != id);

                if (existingUser != null)
                {
                    return ApiResponse<UserDto>.ErrorResponse("El username o email ya están en uso");
                }

                user.Username = updateUserDto.Username;
                user.Email = updateUserDto.Email;
                if (!string.IsNullOrEmpty(updateUserDto.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
                }

                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    IsActive = user.IsActive
                };

                return ApiResponse<UserDto>.SuccessResponse(userDto, "Usuario actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<UserDto>.ErrorResponse($"Error al actualizar usuario: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Usuario no encontrado");
                }

                // Soft delete - marcar como inactivo
                user.IsActive = false;
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Usuario eliminado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al eliminar usuario: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TaskStatisticsDto>> GetUserStatisticsAsync(int userId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.Category)
                    .Where(t => t.UserId == userId)
                    .ToListAsync();

                var totalTasks = tasks.Count;
                var completedTasks = tasks.Count(t => t.IsCompleted);
                var pendingTasks = totalTasks - completedTasks;
                var overdueTasks = tasks.Count(t => t.DueDate < DateTime.UtcNow && !t.IsCompleted);
                var completionRate = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

                // Estadísticas por categoría
                var tasksByCategory = tasks
                    .GroupBy(t => t.Category?.Name ?? "Sin categoría")
                    .ToDictionary(g => g.Key, g => g.Count());

                // Estadísticas por prioridad
                var tasksByPriority = tasks
                    .GroupBy(t => t.Priority)
                    .ToDictionary(g => g.Key, g => g.Count());

                var statistics = new TaskStatisticsDto
                {
                    TotalTasks = totalTasks,
                    CompletedTasks = completedTasks,
                    PendingTasks = pendingTasks,
                    OverdueTasks = overdueTasks,
                    CompletionRate = Math.Round(completionRate, 2),
                    TasksByCategory = tasksByCategory,
                    TasksByPriority = tasksByPriority
                };

                return ApiResponse<TaskStatisticsDto>.SuccessResponse(statistics);
            }
            catch (Exception ex)
            {
                return ApiResponse<TaskStatisticsDto>.ErrorResponse($"Error al obtener estadísticas: {ex.Message}");
            }
        }
    }
}