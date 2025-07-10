using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(7)]
        public string Color { get; set; } = "#3498db";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}