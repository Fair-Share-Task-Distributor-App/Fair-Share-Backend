using Fair_Share_Backend.Data;
using Fair_Share_Backend.DTOs.Team;
using Fair_Share_Backend.Entities;
using Fair_Share_Backend.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share_Backend.Services
{
    public class TeamService
    {
        private readonly ApplicationDbContext _context;
        private readonly TeamMapper _mapper;
        private readonly ILogger<TeamService> _logger;

        public TeamService(
            ApplicationDbContext context,
            TeamMapper mapper,
            ILogger<TeamService> logger
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<TeamResponseDto> CreateTeamAsync(CreateTeamRequestDto request)
        {
            var team = _mapper.ToEntity(request);

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            // Add members if provided
            if (request.MemberIds != null && request.MemberIds.Count > 0)
            {
                foreach (var accountId in request.MemberIds)
                {
                    var accountExists = await _context.Accounts.AnyAsync(a => a.Id == accountId);
                    if (accountExists)
                    {
                        _context.TeamAccounts.Add(
                            new TeamAccount { TeamId = team.Id, AccountId = accountId }
                        );
                    }
                }
                await _context.SaveChangesAsync();
            }

            // Reload with relationships
            team = await _context
                .Teams.Include(t => t.TeamAccounts)
                .ThenInclude(ta => ta.Account)
                .FirstAsync(t => t.Id == team.Id);

            _logger.LogInformation("Team created: {TeamId} - {Name}", team.Id, team.Name);

            return _mapper.ToDto(team);
        }

        public async Task<TeamResponseDto?> GetTeamByIdAsync(int id)
        {
            var team = await _context
                .Teams.Include(t => t.TeamAccounts)
                .ThenInclude(ta => ta.Account)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
            {
                _logger.LogWarning("Team not found: {TeamId}", id);
                return null;
            }

            return _mapper.ToDto(team);
        }

        public async Task<List<TeamResponseDto>> GetAllTeamsAsync()
        {
            var teams = await _context
                .Teams.Include(t => t.TeamAccounts)
                .ThenInclude(ta => ta.Account)
                .ToListAsync();

            return _mapper.ToDtoList(teams);
        }

        public async Task<List<TeamResponseDto>> GetTeamsByAccountIdAsync(int accountId)
        {
            var teams = await _context
                .Teams.Include(t => t.TeamAccounts)
                .ThenInclude(ta => ta.Account)
                .Where(t => t.TeamAccounts.Any(ta => ta.AccountId == accountId))
                .ToListAsync();

            return _mapper.ToDtoList(teams);
        }

        public async Task<TeamResponseDto?> UpdateTeamAsync(int id, UpdateTeamRequestDto request)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                _logger.LogWarning("Team not found for update: {TeamId}", id);
                return null;
            }

            _mapper.UpdateEntity(team, request);
            await _context.SaveChangesAsync();

            // Reload with relationships
            team = await _context
                .Teams.Include(t => t.TeamAccounts)
                .ThenInclude(ta => ta.Account)
                .FirstAsync(t => t.Id == id);

            _logger.LogInformation("Team updated: {TeamId}", id);

            return _mapper.ToDto(team);
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);

            if (team == null)
            {
                _logger.LogWarning("Team not found for deletion: {TeamId}", id);
                return false;
            }

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Team deleted: {TeamId}", id);

            return true;
        }

        public async Task<TeamResponseDto?> AddMembersAsync(
            int teamId,
            AddTeamMembersRequestDto request
        )
        {
            var team = await _context
                .Teams.Include(t => t.TeamAccounts)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
            {
                _logger.LogWarning("Team not found: {TeamId}", teamId);
                return null;
            }

            foreach (var accountId in request.AccountIds)
            {
                // Check if already a member
                var alreadyMember = team.TeamAccounts.Any(ta => ta.AccountId == accountId);
                if (alreadyMember)
                    continue;

                // Check if account exists
                var accountExists = await _context.Accounts.AnyAsync(a => a.Id == accountId);
                if (!accountExists)
                {
                    _logger.LogWarning("Account not found: {AccountId}", accountId);
                    continue;
                }

                _context.TeamAccounts.Add(
                    new TeamAccount { TeamId = teamId, AccountId = accountId }
                );
            }

            await _context.SaveChangesAsync();

            // Reload with updated relationships
            team = await _context
                .Teams.Include(t => t.TeamAccounts)
                .ThenInclude(ta => ta.Account)
                .FirstAsync(t => t.Id == teamId);

            _logger.LogInformation(
                "Added {Count} members to team {TeamId}",
                request.AccountIds.Count,
                teamId
            );

            return _mapper.ToDto(team);
        }

        public async Task<TeamResponseDto?> RemoveMemberAsync(int teamId, int accountId)
        {
            var teamAccount = await _context.TeamAccounts.FirstOrDefaultAsync(ta =>
                ta.TeamId == teamId && ta.AccountId == accountId
            );

            if (teamAccount == null)
            {
                _logger.LogWarning(
                    "Member not found in team: TeamId={TeamId}, AccountId={AccountId}",
                    teamId,
                    accountId
                );
                return null;
            }

            _context.TeamAccounts.Remove(teamAccount);
            await _context.SaveChangesAsync();

            // Reload team with relationships
            var team = await _context
                .Teams.Include(t => t.TeamAccounts)
                .ThenInclude(ta => ta.Account)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return null;

            _logger.LogInformation(
                "Removed account {AccountId} from team {TeamId}",
                accountId,
                teamId
            );

            return _mapper.ToDto(team);
        }
    }
}
