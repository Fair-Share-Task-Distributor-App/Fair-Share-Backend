using Fair_Share.Api.DTOs.Team;
using Fair_Share.Api.Entities;

namespace Fair_Share.Api.Mappers
{
    public class TeamMapper
    {
        public Entities.Team ToEntity(CreateTeamRequestDto dto)
        {
            return new Entities.Team { Name = dto.Name };
        }

        public void UpdateEntity(Entities.Team team, UpdateTeamRequestDto dto)
        {
            if (dto.Name != null)
                team.Name = dto.Name;
        }

        public TeamResponseDto ToDto(Entities.Team team)
        {
            return new TeamResponseDto
            {
                Id = team.Id,
                Name = team.Name,
                Members = team
                    .Accounts.Select(ta => new TeamMemberDto
                    {
                        AccountId = ta.Id,
                        Name = ta.Name,
                        Email = ta.Email
                    })
                    .ToList()
            };
        }

        public List<TeamResponseDto> ToDtoList(IEnumerable<Entities.Team> teams)
        {
            return teams.Select(ToDto).ToList();
        }
    }
}
