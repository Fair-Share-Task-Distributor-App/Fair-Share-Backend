namespace Fair_Share.Api.DTOs.Team
{
    public record TeamResponseDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = null!;
        public List<TeamMemberDto> Members { get; init; } = new();
    }

    public record TeamMemberDto
    {
        public int AccountId { get; init; }
        public string Name { get; init; } = null!;
        public string Email { get; init; } = null!;
    }
}
