using System.ComponentModel.DataAnnotations;

namespace Fair_Share_Backend.DTOs.Task
{
    public record AssignTaskRequestDto
    {
        [Required]
        public List<int> AccountIds { get; init; } = new();
    }
}
