using Fair_Share.Api.Data;
using Fair_Share.Api.DTOs.Team;
using Fair_Share.Api.Entities;
using Fair_Share.Api.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share.Api.Services
{
    public class TeamService
    {
        private readonly ApplicationDbContext _context;
        private readonly TeamMapper _mapper;
        private readonly ILogger<TeamService> _logger;
        private readonly IConfiguration _configuration;

        public TeamService(
            ApplicationDbContext context,
            TeamMapper mapper,
            ILogger<TeamService> logger,
            IConfiguration configuration
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<CreateTeamResult> CreateTeamAsync(
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

            _mapper.ToDto(team);

            // Update JWT
            var jwtKey =
                _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured");
            var jwtIssuer =
                _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer not configured");
            var jwtAudience =
                _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience not configured");
            var token = JwtGenerator.GenerateJwtToken(account, jwtKey, jwtIssuer, jwtAudience);

            return new CreateTeamResult { Team = _mapper.ToDto(team), Jwt = token };
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

        public async Task<List<TeamMemberDto>> AddMembersAsync(
            int teamId,
            AddTeamMembersRequestDto request
        )
        {
            var team = await _context
                .Teams.Include(t => t.Accounts)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            if (team == null)
                return new List<TeamMemberDto>();

            var addedMembers = new List<TeamMemberDto>();

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
                addedMembers.Add(
                    new TeamMemberDto
                    {
                        AccountId = account.Id,
                        Name = account.Name,
                        Email = account.Email
                    }
                );
            }

            await _context.SaveChangesAsync();

            return addedMembers;
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

            account.TeamId = null; // Removing the account from the team by setting to null

            // Remove all task assignments for this user
            var accountTasks = await _context
                .AccountTasks.Where(at => at.AccountId == accountId)
                .ToListAsync();
            _context.AccountTasks.RemoveRange(accountTasks);

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
