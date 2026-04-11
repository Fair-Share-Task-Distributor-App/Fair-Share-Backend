using System.ComponentModel.DataAnnotations;

namespace Fair_Share.Api.DTOs.Team
{
    public record CreateTeamRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; init; } = null!;
    }
}
