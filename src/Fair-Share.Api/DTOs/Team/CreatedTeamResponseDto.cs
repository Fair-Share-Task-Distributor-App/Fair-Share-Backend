namespace Fair_Share.Api.DTOs.Team
{
    public record CreateTeamResult
    {
        public TeamResponseDto Team { get; set; } = null!;
        public string Jwt { get; set; } = null!;
    }
}
