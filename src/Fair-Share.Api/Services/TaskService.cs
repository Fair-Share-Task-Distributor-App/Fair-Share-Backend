using Azure.Messaging.ServiceBus;
using Fair_Share.Api.Data;
using Fair_Share.Api.DTOs.Task;
using Fair_Share.Api.Entities;
using Fair_Share.Api.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share.Api.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly TaskMapper _mapper;
        private readonly ILogger<TaskService> _logger;
        private readonly ServiceBusSender _serviceBusSender;

        public TaskService(
            ApplicationDbContext context,
            TaskMapper mapper,
            ILogger<TaskService> logger,
            ServiceBusSender serviceBusSender
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _serviceBusSender = serviceBusSender;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskRequestDto request)
        {
            var task = _mapper.ToEntity(request);

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Reload with relationships
            task = await _context
                .Tasks.Include(t => t.AccountTasks)
                .ThenInclude(at => at.Account)
                .FirstAsync(t => t.Id == task.Id);

            _logger.LogInformation("Task created: {TaskId} - {Title}", task.Id, task.Title);

            var messagePayload = System.Text.Json.JsonSerializer.Serialize(
                new
                {
                    TaskId = task.Id,
                    PointsWorth = task.Points,
                    Time = task.AutoAssignAt
                }
            );
            await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(messagePayload));

            return _mapper.ToDto(task);
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int id)
        {
            var task = await _context
                .Tasks.Include(t => t.AccountTasks)
                .ThenInclude(at => at.Account)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                _logger.LogWarning("Task not found: {TaskId}", id);
                return null;
            }

            return _mapper.ToDto(task);
        }

        public async Task<List<TaskResponseDto>> GetTasksInTeamAsync(int teamId)
        {
            var team = await _context
                .Teams.Include(t => t.Tasks)
                .ThenInclude(t => t.AccountTasks)
                .ThenInclude(at => at.Account)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            var tasks = team?.Tasks.ToList();

            return tasks != null
                ? tasks.Select(t => _mapper.ToDto(t)).ToList()
                : new List<TaskResponseDto>();
        }

        public async Task<List<TaskResponseDto>> GetUnassignedTasksInTeamAsync(
            int accountId,
            int teamId
        )
        {
            var team = await _context
                .Teams.Include(t => t.Tasks)
                .ThenInclude(t => t.AccountTasks)
                .ThenInclude(at => at.Account)
                .Include(t => t.Tasks)
                .ThenInclude(t => t.AccountTaskPreferences.Where(p => p.AccountId == accountId))
                .FirstOrDefaultAsync(t => t.Id == teamId);

            var unassignedTasks = team?.Tasks.Where(t => t.AccountTasks.Count == 0).ToList();

            var unassignedTasksDtos =
                unassignedTasks != null
                    ? unassignedTasks.Select(t => _mapper.ToDto(t)).ToList()
                    : new List<TaskResponseDto>();

            // set it to 5 if no preference exists
            foreach (var dto in unassignedTasksDtos)
            {
                dto.UserPreferenceRating = dto.UserPreferenceRating ?? 5;
            }

            return unassignedTasksDtos;
        }

        public async Task<List<TaskResponseDto>> GetTasksForAccountAsync(int accountId)
        {
            var tasks = await _context
                .Tasks.Where(t => t.AccountTasks.Any(at => at.AccountId == accountId))
                .Include(t => t.AccountTasks)
                .ThenInclude(at => at.Account)
                .ToListAsync();

            return tasks.Select(t => _mapper.ToDto(t)).ToList();
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(
            int id,
            int teamId,
            UpdateTaskRequestDto request
        )
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t =>
                t.Id == id && t.TeamOwnedId == teamId
            );

            if (task == null)
            {
                _logger.LogWarning("Task not found for update: {TaskId}", id);
                return null;
            }

            _mapper.UpdateEntity(task, request);
            await _context.SaveChangesAsync();

            // If the auto assign date is changed, queue another task with new date
            if (request.AutoAssignAt != null)
            {
                var messagePayload = System.Text.Json.JsonSerializer.Serialize(
                    new
                    {
                        TaskId = task.Id,
                        PointsWorth = task.Points,
                        Time = task.AutoAssignAt
                    }
                );
                await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(messagePayload));
            }

            // Reload with relationships
            task = await _context
                .Tasks.Include(t => t.AccountTasks)
                .ThenInclude(at => at.Account)
                .FirstAsync(t => t.Id == id);

            _logger.LogInformation("Task updated: {TaskId}", id);

            return _mapper.ToDto(task);
        }

        public async Task<bool> DeleteTaskAsync(int id, int teamId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t =>
                t.Id == id && t.TeamOwnedId == teamId
            );

            if (task == null)
            {
                _logger.LogWarning("Task not found for deletion: {TaskId}", id);
                return false;
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Task deleted: {TaskId}", id);

            return true;
        }

        public async Task<TaskResponseDto?> AssignTaskToAccountsAsync(
            int taskId,
            AssignTaskRequestDto request
        )
        {
            var task = await _context
                .Tasks.Include(t => t.AccountTasks)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                _logger.LogWarning("Task not found for assignment: {TaskId}", taskId);
                return null;
            }

            // Remove existing assignments
            _context.AccountTasks.RemoveRange(task.AccountTasks);

            // Add new assignments
            foreach (var accountId in request.AccountIds)
            {
                var accountExists = await _context.Accounts.AnyAsync(a => a.Id == accountId);
                if (!accountExists)
                {
                    _logger.LogWarning("Account not found for assignment: {AccountId}", accountId);
                    continue;
                }

                task.AccountTasks.Add(
                    new AccountTask
                    {
                        TaskId = taskId,
                        AccountId = accountId,
                        AssignedAt = DateTime.UtcNow
                    }
                );
            }

            await _context.SaveChangesAsync();

            // Reload with updated relationships
            task = await _context
                .Tasks.Include(t => t.AccountTasks)
                .ThenInclude(at => at.Account)
                .FirstAsync(t => t.Id == taskId);

            _logger.LogInformation(
                "Task {TaskId} assigned to {Count} accounts",
                taskId,
                request.AccountIds.Count
            );

            return _mapper.ToDto(task);
        }
    }
}
