using Fair_Share_Backend.DTOs.Team;
using Fair_Share_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Fair_Share_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamController :ControllerBase
    {
        private readonly TeamService _teamService;
        private readonly ILogger<TeamController> _logger;

        public TeamController(TeamService teamService, ILogger<TeamController> logger)
        {
            _teamService = teamService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequestDto request)
        {
            var result = await _teamService.CreateTeamAsync(request);
            return CreatedAtAction(nameof(GetTeamById), new
            {
                id = result.Id
            }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeamById(int id)
        {
            var result = await _teamService.GetTeamByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = $"Team with ID {id} not found"
                });
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTeams()
        {
            var results = await _teamService.GetAllTeamsAsync();
            return Ok(results);
        }

        [HttpGet("myTeams")]
        public async Task<IActionResult> GetTeamsByAccountId()
        {
            var accountId = int.Parse(
                User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)!.Value
            );
            var results = await _teamService.GetTeamsByAccountIdAsync(accountId);
            return Ok(results);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] UpdateTeamRequestDto request)
        {
            var result = await _teamService.UpdateTeamAsync(id, request);

            if (result == null)
            {
                return NotFound(new
                {
                    message = $"Team with ID {id} not found"
                });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var success = await _teamService.DeleteTeamAsync(id);

            if (!success)
            {
                return NotFound(new
                {
                    message = $"Team with ID {id} not found"
                });
            }

            return NoContent();
        }

        [HttpPost("{id}/members")]
        public async Task<IActionResult> AddMembers(
            int id,
            [FromBody] AddTeamMembersRequestDto request
        )
        {
            var result = await _teamService.AddMembersAsync(id, request);

            if (result == null)
            {
                return NotFound(new
                {
                    message = $"Team with ID {id} not found"
                });
            }

            return Ok(result);
        }

        //[HttpDelete("{teamId}/members/{accountId}")]
        //public async Task<IActionResult> RemoveMember(int teamId, int accountId)
        //{
        //    var result = await _teamService.RemoveMemberAsync(teamId, accountId);

        //    if (result == null)
        //    {
        //        return NotFound(new
        //        {
        //            message = $"Member {accountId} not found in team {teamId}"
        //        });
        //    }

        //    return Ok(result);
        //}
    }
}
