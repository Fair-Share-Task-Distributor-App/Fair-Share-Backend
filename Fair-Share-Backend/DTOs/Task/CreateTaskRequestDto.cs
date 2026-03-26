using System.ComponentModel.DataAnnotations;

namespace Fair_Share_Backend.DTOs.Task
{
    public record CreateTaskRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; init; } = null!;

        [MaxLength(1000)]
        public string? Description { get; init; }

        [Required]
        public DateTime DueAt { get; init; }

        [Required]
        public DateTime AutoAssignAt { get; init; }

        [Required]
        [Range(1, 100)]
        public int Points { get; init; }

        public int TeamId { get; set; }
    }
}
