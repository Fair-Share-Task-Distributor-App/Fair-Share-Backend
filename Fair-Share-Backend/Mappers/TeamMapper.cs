using Fair_Share_Backend.DTOs.Team;
using Fair_Share_Backend.Entities;

namespace Fair_Share_Backend.Mappers
{
    public class TeamMapper
    {
        public Entities.Team ToEntity(CreateTeamRequestDto dto)
        {
            return new Entities.Team
            {
                Name = dto.Name
            };
        }

        public void UpdateEntity(Entities.Team team, UpdateTeamRequestDto dto)
        {
            if (dto.Name != null) team.Name = dto.Name;
        }

        public TeamResponseDto ToDto(Entities.Team team)
        {
            return new TeamResponseDto
            {
                Id = team.Id,
                Name = team.Name,
                Members = team.TeamAccounts.Select(ta => new TeamMemberDto
                {
                    AccountId = ta.AccountId,
                    Name = ta.Account.Name,
                    Email = ta.Account.Email
                }).ToList()
            };
        }

        public List<TeamResponseDto> ToDtoList(IEnumerable<Entities.Team> teams)
        {
            return teams.Select(ToDto).ToList();
        }
    }
}
