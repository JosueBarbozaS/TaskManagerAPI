using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Interfaces
{
    public interface ITaskService
    {
        Task<ApiResponse<List<TaskDto>>> GetTasksByUserAsync(int userId);
        Task<ApiResponse<TaskDto>> GetTaskByIdAsync(int id, int userId);
        Task<ApiResponse<TaskDto>> CreateTaskAsync(CreateTaskDto createTaskDto, int userId);
        Task<ApiResponse<TaskDto>> UpdateTaskAsync(int id, UpdateTaskDto updateTaskDto, int userId);
        Task<ApiResponse<bool>> DeleteTaskAsync(int id, int userId);
        Task<ApiResponse<bool>> ToggleTaskStatusAsync(int id, int userId);
        Task<ApiResponse<List<TaskDto>>> GetOverdueTasksAsync(int userId);
        Task<ApiResponse<List<TaskDto>>> GetTasksByPriorityAsync(int userId, int priority);
    }
}