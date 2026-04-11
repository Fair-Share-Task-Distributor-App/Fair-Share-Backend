using Fair_Share.Api.DTOs.TaskPreference;
using Fair_Share.Api.Entities;

namespace Fair_Share.Api.Mappers
{
    public class TaskPreferenceMapper
    {
        public AccountTaskPreference ToEntity(CreateTaskPreferenceRequestDto dto)
        {
            return new AccountTaskPreference
            {
                AccountId = dto.AccountId,
                TaskId = dto.TaskId,
                Score = dto.Score
            };
        }

        public void UpdateEntity(
            AccountTaskPreference preference,
            UpdateTaskPreferenceRequestDto dto
        )
        {
            preference.Score = dto.Score;
        }

        public TaskPreferenceResponseDto ToDto(AccountTaskPreference preference)
        {
            return new TaskPreferenceResponseDto
            {
                TaskId = preference.TaskId,
                Score = preference.Score,
            };
        }

        public List<TaskPreferenceResponseDto> ToDtoList(
            IEnumerable<AccountTaskPreference> preferences
        )
        {
            return preferences.Select(ToDto).ToList();
        }
    }
}
