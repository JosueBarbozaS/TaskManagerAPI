using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = "#3498db";
        public DateTime CreatedAt { get; set; }
        public int TaskCount { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(7)]
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Color must be a valid hex color code")]
        public string Color { get; set; } = "#3498db";
    }
}