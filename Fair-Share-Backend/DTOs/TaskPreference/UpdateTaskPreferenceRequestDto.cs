using System.ComponentModel.DataAnnotations;

namespace Fair_Share_Backend.DTOs.TaskPreference
{
    public record UpdateTaskPreferenceRequestDto
    {
        [Required]
        [Range(1, 10)]
        public int Score { get; init; }
    }
}
