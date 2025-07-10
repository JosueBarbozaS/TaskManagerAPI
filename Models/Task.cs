// Models/Task.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerAPI.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        [Range(1, 3)]
        public int Priority { get; set; } = 1; // 1=Baja, 2=Media, 3=Alta

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public int UserId { get; set; }

        public int? CategoryId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}