using Fair_Share_Backend.Data;
using Fair_Share_Backend.DTOs.Account;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MyAccountResponseDto?> GetMyAccountAsync(int accountId)
        {
            var account = await _context.Accounts
                .Include(a => a.AccountTasks)
                .ThenInclude(at => at.Task)
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                return null;
            }

            var assignedTasksCount = account.AccountTasks.Count(at => !at.Task.IsCompleted);

            return new MyAccountResponseDto
            {
                Name = account.Name,
                Email = account.Email,
                Password = account.Password,
                Points = account.Points,
                TasksAssigned = assignedTasksCount
            };
        }
    }
}