namespace Fair_Share_Backend.DTOs.Team
{
    public record CreateTeamResult
    {
        public TeamResponseDto Team { get; set; } = null!;
        public string Jwt { get; set; } = null!;
    }
}
