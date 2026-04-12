using System.ComponentModel.DataAnnotations;

namespace Fair_Share.Api.DTOs.Team
{
    public record AddTeamMembersRequestDto
    {
        [Required]
        public List<string> Emails { get; init; } = new();
    }
}
