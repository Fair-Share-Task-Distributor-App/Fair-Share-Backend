namespace Fair_Share_Backend.DTOs.TaskPreference
{
    public record TaskPreferenceResponseDto
    {
        public int TaskId { get; init; }
        public int Score { get; init; }
    }
}
