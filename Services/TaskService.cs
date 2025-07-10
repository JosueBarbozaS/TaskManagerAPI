using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Interfaces;

namespace TaskManagerAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskManagerDbContext _context;

        public TaskService(TaskManagerDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<TaskDto>>> GetTasksByUserAsync(int userId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.Category)
                    .Where(t => t.UserId == userId)
                    .OrderBy(t => t.Priority)
                    .ThenBy(t => t.DueDate)
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        IsCompleted = t.IsCompleted,
                        Priority = t.Priority,
                        DueDate = t.DueDate,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt,
                        UserId = t.UserId,
                        CategoryId = t.CategoryId,
                        CategoryName = t.Category != null ? t.Category.Name : null,
                        CategoryColor = t.Category != null ? t.Category.Color : null
                    })
                    .ToListAsync();

                return ApiResponse<List<TaskDto>>.SuccessResponse(tasks);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<TaskDto>>.ErrorResponse($"Error al obtener tareas: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TaskDto>> GetTaskByIdAsync(int id, int userId)
        {
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                if (task == null)
                {
                    return ApiResponse<TaskDto>.ErrorResponse("Tarea no encontrada");
                }

                var taskDto = new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    IsCompleted = task.IsCompleted,
                    Priority = task.Priority,
                    DueDate = task.DueDate,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt,
                    UserId = task.UserId,
                    CategoryId = task.CategoryId,
                    CategoryName = task.Category?.Name,
                    CategoryColor = task.Category?.Color
                };

                return ApiResponse<TaskDto>.SuccessResponse(taskDto);
            }
            catch (Exception ex)
            {
                return ApiResponse<TaskDto>.ErrorResponse($"Error al obtener tarea: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TaskDto>> CreateTaskAsync(CreateTaskDto createTaskDto, int userId)
        {
            try
            {
                var task = new Models.Task
                {
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    Priority = createTaskDto.Priority,
                    DueDate = createTaskDto.DueDate,
                    CategoryId = createTaskDto.CategoryId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                // Obtener la tarea creada con la categoría
                var createdTask = await _context.Tasks
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.Id == task.Id);

                var taskDto = new TaskDto
                {
                    Id = createdTask.Id,
                    Title = createdTask.Title,
                    Description = createdTask.Description,
                    IsCompleted = createdTask.IsCompleted,
                    Priority = createdTask.Priority,
                    DueDate = createdTask.DueDate,
                    CreatedAt = createdTask.CreatedAt,
                    UpdatedAt = createdTask.UpdatedAt,
                    UserId = createdTask.UserId,
                    CategoryId = createdTask.CategoryId,
                    CategoryName = createdTask.Category?.Name,
                    CategoryColor = createdTask.Category?.Color
                };

                return ApiResponse<TaskDto>.SuccessResponse(taskDto, "Tarea creada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<TaskDto>.ErrorResponse($"Error al crear tarea: {ex.Message}");
            }
        }

        public async Task<ApiResponse<TaskDto>> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId)
        {
            try
            {
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                if (task == null)
                {
                    return ApiResponse<TaskDto>.ErrorResponse("Tarea no encontrada");
                }

                task.Title = updateTaskDto.Title;
                task.Description = updateTaskDto.Description;
                task.IsCompleted = updateTaskDto.IsCompleted;
                task.Priority = updateTaskDto.Priority;
                task.DueDate = updateTaskDto.DueDate;
                task.CategoryId = updateTaskDto.CategoryId;
                task.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Obtener la tarea actualizada con la categoría
                var updatedTask = await _context.Tasks
                    .Include(t => t.Category)
                    .FirstOrDefaultAsync(t => t.Id == task.Id);

                var taskDto = new TaskDto
                {
                    Id = updatedTask.Id,
                    Title = updatedTask.Title,
                    Description = updatedTask.Description,
                    IsCompleted = updatedTask.IsCompleted,
                    Priority = updatedTask.Priority,
                    DueDate = updatedTask.DueDate,
                    CreatedAt = updatedTask.CreatedAt,
                    UpdatedAt = updatedTask.UpdatedAt,
                    UserId = updatedTask.UserId,
                    CategoryId = updatedTask.CategoryId,
                    CategoryName = updatedTask.Category?.Name,
                    CategoryColor = updatedTask.Category?.Color
                };

                return ApiResponse<TaskDto>.SuccessResponse(taskDto, "Tarea actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<TaskDto>.ErrorResponse($"Error al actualizar tarea: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteTaskAsync(int id, int userId)
        {
            try
            {
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                if (task == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Tarea no encontrada");
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Tarea eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al eliminar tarea: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> ToggleTaskStatusAsync(int id, int userId)
        {
            try
            {
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

                if (task == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Tarea no encontrada");
                }

                task.IsCompleted = !task.IsCompleted;
                task.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResponse(true, "Estado de tarea actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Error al cambiar estado de tarea: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<TaskDto>>> GetOverdueTasksAsync(int userId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.Category)
                    .Where(t => t.UserId == userId && t.DueDate < DateTime.UtcNow && !t.IsCompleted)
                    .OrderBy(t => t.DueDate)
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        IsCompleted = t.IsCompleted,
                        Priority = t.Priority,
                        DueDate = t.DueDate,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt,
                        UserId = t.UserId,
                        CategoryId = t.CategoryId,
                        CategoryName = t.Category != null ? t.Category.Name : null,
                        CategoryColor = t.Category != null ? t.Category.Color : null
                    })
                    .ToListAsync();

                return ApiResponse<List<TaskDto>>.SuccessResponse(tasks);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<TaskDto>>.ErrorResponse($"Error al obtener tareas vencidas: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<TaskDto>>> GetTasksByPriorityAsync(int userId, int priority)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Include(t => t.Category)
                    .Where(t => t.UserId == userId && t.Priority == priority)
                    .OrderBy(t => t.DueDate)
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        IsCompleted = t.IsCompleted,
                        Priority = t.Priority,
                        DueDate = t.DueDate,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt,
                        UserId = t.UserId,
                        CategoryId = t.CategoryId,
                        CategoryName = t.Category != null ? t.Category.Name : null,
                        CategoryColor = t.Category != null ? t.Category.Color : null
                    })
                    .ToListAsync();

                return ApiResponse<List<TaskDto>>.SuccessResponse(tasks);
            }
            catch (Exception ex)
            {
                return ApiResponse<List<TaskDto>>.ErrorResponse($"Error al obtener tareas por prioridad: {ex.Message}");
            }
        }
    }
}