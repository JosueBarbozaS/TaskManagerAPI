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
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst("id")?.Value ?? "0");
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<TaskDto>>>> GetTasks()
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetTasksByUserAsync(userId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TaskDto>>> GetTask(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetTaskByIdAsync(id, userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TaskDto>>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<TaskDto>.ErrorResponse("Datos inválidos", errors));
            }

            var userId = GetCurrentUserId();
            var result = await _taskService.CreateTaskAsync(createTaskDto, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetTask), new { id = result.Data?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TaskDto>>> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(ApiResponse<TaskDto>.ErrorResponse("Datos inválidos", errors));
            }

            var userId = GetCurrentUserId();
            var result = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.DeleteTaskAsync(id, userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPatch("{id}/toggle")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleTaskStatus(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.ToggleTaskStatusAsync(id, userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("overdue")]
        public async Task<ActionResult<ApiResponse<List<TaskDto>>>> GetOverdueTasks()
        {
            var userId = GetCurrentUserId();
            var result = await _taskService.GetOverdueTasksAsync(userId);
            return Ok(result);
        }

        [HttpGet("priority/{priority}")]
        public async Task<ActionResult<ApiResponse<List<TaskDto>>>> GetTasksByPriority(int priority)
        {
            if (priority < 1 || priority > 3)
            {
                return BadRequest(ApiResponse<List<TaskDto>>.ErrorResponse("La prioridad debe estar entre 1 y 3"));
            }

            var userId = GetCurrentUserId();
            var result = await _taskService.GetTasksByPriorityAsync(userId, priority);
            return Ok(result);
        }
    }
}