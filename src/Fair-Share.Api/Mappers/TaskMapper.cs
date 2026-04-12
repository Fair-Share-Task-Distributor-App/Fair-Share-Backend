using Fair_Share.Api.DTOs.Task;
using Fair_Share.Api.Entities;

namespace Fair_Share.Api.Mappers
{
    public class TaskMapper
    {
        public Entities.Task ToEntity(CreateTaskRequestDto dto)
        {
            return new Entities.Task
            {
                Title = dto.Title,
                Description = dto.Description,
                DueAt = dto.DueAt,
                AutoAssignAt = dto.AutoAssignAt,
                Points = dto.Points,
                IsCompleted = false,
                TeamOwnedId = dto.TeamId
            };
        }

        public void UpdateEntity(Entities.Task task, UpdateTaskRequestDto dto)
        {
            if (dto.Title != null)
                task.Title = dto.Title;
            if (dto.Description != null)
                task.Description = dto.Description;
            if (dto.DueAt.HasValue)
                task.DueAt = dto.DueAt.Value;
            if (dto.AutoAssignAt.HasValue)
                task.AutoAssignAt = dto.AutoAssignAt.Value;
            if (dto.Points.HasValue)
                task.Points = dto.Points.Value;
            if (dto.IsCompleted.HasValue)
                task.IsCompleted = dto.IsCompleted.Value;
        }

        public TaskResponseDto ToDto(Entities.Task task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueAt = task.DueAt,
                AutoAssignAt = task.AutoAssignAt,
                IsCompleted = task.IsCompleted,
                Points = task.Points,
                UserPreferenceRating = task.AccountTaskPreferences?.FirstOrDefault()?.Score,
                AssignedAccounts = task
                    .AccountTasks.Select(at => new AssignedAccountDto
                    {
                        AccountId = at.AccountId,
                        Name = at.Account.Name,
                        Email = at.Account.Email,
                        AssignedAt = at.AssignedAt
                    })
                    .ToList()
            };
        }

        public List<TaskResponseDto> ToDtoList(IEnumerable<Entities.Task> tasks)
        {
            return tasks.Select(ToDto).ToList();
        }
    }
}
