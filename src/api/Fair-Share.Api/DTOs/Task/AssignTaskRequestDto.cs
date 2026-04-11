using System.ComponentModel.DataAnnotations;

namespace Fair_Share.Api.DTOs.Task
{
    public record AssignTaskRequestDto
    {
        [Required]
        public List<int> AccountIds { get; init; } = new();
    }
}
