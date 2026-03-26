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

        public async Task<TeamResponseDto> CreateTeamAsync(
            CreateTeamRequestDto request,
            int teamOwnerId
        )
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == teamOwnerId);

            if (account == null)
                throw new Exception("User not found");

            var team = _mapper.ToEntity(request);

            _context.Teams.Add(team);
            account.Team = team;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Team created: {TeamId} - {Name}", team.Id, team.Name);

            return _mapper.ToDto(team);
        }

        public async Task<TeamResponseDto?> GetTeamByIdAsync(int id)
        {
            var team = await _context
                .Teams.Include(t => t.Accounts)
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
            var teams = await _context.Teams.Include(t => t.Accounts).ToListAsync();

            return _mapper.ToDtoList(teams);
        }

        public async Task<List<TeamResponseDto>> GetTeamsByAccountIdAsync(int accountId)
        {
            var teams = await _context
                .Teams.Include(t => t.Accounts)
                .Where(t => t.Accounts.Any(a => a.Id == accountId))
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
            team = await _context.Teams.Include(t => t.Accounts).FirstAsync(t => t.Id == id);

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
                .Teams.Include(t => t.Accounts)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            foreach (var email in request.Emails)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

                if (account == null)
                {
                    _logger.LogWarning("Account not found: {Email}", email);
                    continue;
                }

                // Check if already a member of this team
                if (account.TeamId == teamId)
                    continue;

                // Check if already a member of another team
                if (account.TeamId != null)
                {
                    _logger.LogWarning(
                        "Account {Email} is already a member of another team (TeamId={ExistingTeamId})",
                        email,
                        account.TeamId
                    );
                    continue;
                }

                account.TeamId = teamId;
            }

            await _context.SaveChangesAsync();

            // Reload with updated relationships
            team = await _context.Teams.Include(t => t.Accounts).FirstAsync(t => t.Id == teamId);

            return _mapper.ToDto(team);
        }

        public async Task<TeamResponseDto?> RemoveMemberAsync(int teamId, int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a =>
                a.TeamId == teamId && a.Id == accountId
            );

            if (account == null)
            {
                _logger.LogWarning(
                    "Member not found in team: TeamId={TeamId}, AccountId={AccountId}",
                    teamId,
                    accountId
                );
                return null;
            }

            account.TeamId = 0; // Removing the account from the team by setting to 0 (or default value)
            await _context.SaveChangesAsync();

            // Reload team with relationships
            var team = await _context
                .Teams.Include(t => t.Accounts)
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
