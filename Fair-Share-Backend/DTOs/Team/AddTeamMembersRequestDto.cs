using System.ComponentModel.DataAnnotations;

namespace Fair_Share_Backend.DTOs.Team
{
    public record AddTeamMembersRequestDto
    {
        [Required]
        public List<int> AccountIds { get; init; } = new();
    }
}
