using System.ComponentModel.DataAnnotations;

namespace Fair_Share.Api.DTOs.TaskPreference
{
    public record CreateTaskPreferenceRequestDto
    {
        [Required]
        public int AccountId { get; init; }

        [Required]
        public int TaskId { get; init; }

        [Required]
        [Range(1, 10)]
        public int Score { get; init; }
    }
}
