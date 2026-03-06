using Fair_Share_Backend.Data;
using Fair_Share_Backend.DTOs.Task;
using Fair_Share_Backend.Entities;
using Fair_Share_Backend.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly TaskMapper _mapper;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            ApplicationDbContext context,
            TaskMapper mapper,
            ILogger<TaskService> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskRequestDto request)
        {
            try
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

                return _mapper.ToDto(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                throw;
            }
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching task {TaskId}", id);
                throw;
            }
        }

        public async Task<List<TaskResponseDto>> GetTasksInTeamAsync(int teamId)
        {
            var team = await _context
                .Teams.Include(t => t.Tasks)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            var tasks = team?.Tasks.ToList();

            return tasks != null
                ? tasks.Select(t => _mapper.ToDto(t)).ToList()
                : new List<TaskResponseDto>();
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(int id, UpdateTaskRequestDto request)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);

                if (task == null)
                {
                    _logger.LogWarning("Task not found for update: {TaskId}", id);
                    return null;
                }

                _mapper.UpdateEntity(task, request);
                await _context.SaveChangesAsync();

                // Reload with relationships
                task = await _context
                    .Tasks.Include(t => t.AccountTasks)
                    .ThenInclude(at => at.Account)
                    .FirstAsync(t => t.Id == id);

                _logger.LogInformation("Task updated: {TaskId}", id);

                return _mapper.ToDto(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", id);
                throw;
            }
        }

        public async Task<TaskResponseDto?> AssignTaskToAccountsAsync(
            int taskId,
            AssignTaskRequestDto request
        )
        {
            try
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
                        _logger.LogWarning(
                            "Account not found for assignment: {AccountId}",
                            accountId
                        );
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task {TaskId}", taskId);
                throw;
            }
        }
    }
}
