using System.ComponentModel.DataAnnotations;

namespace Fair_Share_Backend.DTOs.Task
{
    public record UpdateTaskRequestDto
    {
        [MaxLength(200)]
        public string? Title { get; init; }

        [MaxLength(1000)]
        public string? Description { get; init; }

        public DateTime? DueAt { get; init; }

        public DateTime? AutoAssignAt { get; init; }

        [Range(1, 100)]
        public int? Points { get; init; }

        public bool? IsCompleted { get; init; }
    }
}
