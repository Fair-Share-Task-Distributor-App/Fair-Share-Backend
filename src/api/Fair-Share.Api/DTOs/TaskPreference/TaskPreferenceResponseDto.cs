namespace Fair_Share.Api.DTOs.TaskPreference
{
    public record TaskPreferenceResponseDto
    {
        public int TaskId { get; init; }
        public int Score { get; init; }
    }
}
