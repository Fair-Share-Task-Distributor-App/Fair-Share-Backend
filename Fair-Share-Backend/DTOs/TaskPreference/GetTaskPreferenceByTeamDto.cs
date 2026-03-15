namespace Fair_Share_Backend.DTOs.TaskPreference
{
    public record GetTaskPreferenceByTeamDto
    {
        public int TeamId
        {
            get; init;
        }
    }
}
