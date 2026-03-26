namespace Fair_Share_Backend.DTOs.Task
{
    public record TaskResponseDto
    {
        public int Id { get; init; }
        public string Title { get; init; } = null!;
        public string? Description { get; init; }
        public DateTime DueAt { get; init; }
        public DateTime AutoAssignAt { get; init; }
        public bool IsCompleted { get; init; }
        public int Points { get; init; }

        public int? UserPreferenceRating { get; set; }
        public List<AssignedAccountDto>? AssignedAccounts { get; init; }
    }

    public record AssignedAccountDto
    {
        public int AccountId { get; init; }
        public string Name { get; init; } = null!;
        public string Email { get; init; } = null!;
        public DateTime AssignedAt { get; init; }
    }
}
