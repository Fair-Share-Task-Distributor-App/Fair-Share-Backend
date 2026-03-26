using Fair_Share_Backend.Data;
using Fair_Share_Backend.DTOs.TaskPreference;
using Fair_Share_Backend.Entities;
using Fair_Share_Backend.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Services
{
    public class TaskPreferenceService
    {
        private readonly ApplicationDbContext _context;
        private readonly TaskPreferenceMapper _mapper;
        private readonly ILogger<TaskPreferenceService> _logger;

        public TaskPreferenceService(
            ApplicationDbContext context,
            TaskPreferenceMapper mapper,
            ILogger<TaskPreferenceService> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TaskPreferenceResponseDto?> GetTaskPreferenceAsync(
            int accountId,
            int taskId
        )
        {
            var preference = await _context
                .AccountTaskPreferences.Include(p => p.Account)
                .Include(p => p.Task)
                .FirstOrDefaultAsync(p => p.AccountId == accountId && p.TaskId == taskId);

            if (preference == null)
            {
                _logger.LogWarning(
                    "Task preference not found: AccountId {AccountId}, TaskId {TaskId}",
                    accountId,
                    taskId
                );
                return null;
            }

            return _mapper.ToDto(preference);
        }

        public async Task<List<TaskPreferenceResponseDto>> GetTeamTasksPreferencesForAccountIdAsync(
            int accountId,
            int teamId
        )
        {
            var scores = await _context
                .Database.SqlQuery<TaskPreferenceResponseDto>(
                    $@"SELECT 
                        t.id AS taskId,
                        t.title,
                        COALESCE(p.score, 5) AS score
                    FROM task t
                    LEFT JOIN account_task_preference p
                        ON p.task_id = t.id
                        AND p.account_id = {accountId}
                    WHERE t.team_owned_id = {teamId};
                    "
                )
                .ToListAsync();

            return scores;
        }

        public async Task<TaskPreferenceResponseDto?> UpdateTaskPreferenceAsync(
            int accountId,
            int taskId,
            UpdateTaskPreferenceRequestDto request
        )
        {
            var preference = await _context.AccountTaskPreferences.FirstOrDefaultAsync(p =>
                p.AccountId == accountId && p.TaskId == taskId
            );

            if (preference == null)
            {
                preference = new AccountTaskPreference
                {
                    AccountId = accountId,
                    TaskId = taskId,
                    Score = request.Score
                };
                await _context.AccountTaskPreferences.AddAsync(preference);
                await _context.SaveChangesAsync();
            }
            else
            {
                _mapper.UpdateEntity(preference, request);
                await _context.SaveChangesAsync();
            }

            return _mapper.ToDto(preference);
        }
    }
}
