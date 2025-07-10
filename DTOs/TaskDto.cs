using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public int Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryColor { get; set; }
    }

    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Range(1, 3)]
        public int Priority { get; set; } = 1;

        public DateTime? DueDate { get; set; }

        public int? CategoryId { get; set; }
    }

    public class UpdateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        [Range(1, 3)]
        public int Priority { get; set; } = 1;

        public DateTime? DueDate { get; set; }

        public int? CategoryId { get; set; }
    }
}