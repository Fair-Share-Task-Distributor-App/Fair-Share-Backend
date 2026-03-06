using System.ComponentModel.DataAnnotations;

namespace Fair_Share_Backend.DTOs.Team
{
    public record CreateTeamRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; init; } = null!;

        public List<int>? MemberIds { get; init; }
    }
}
