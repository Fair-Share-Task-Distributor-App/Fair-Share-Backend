using System.ComponentModel.DataAnnotations;

namespace Fair_Share_Backend.DTOs.Team
{
    public record UpdateTeamRequestDto
    {
        [MaxLength(200)]
        public string? Name { get; init; }
    }
}
