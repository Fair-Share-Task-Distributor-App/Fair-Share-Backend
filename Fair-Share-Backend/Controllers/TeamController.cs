using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fair_Share_Backend.DTOs.Team;
using Fair_Share_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fair_Share_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamController : ControllerBase
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
            var accountId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            var result = await _teamService.CreateTeamAsync(request, accountId);
            return CreatedAtAction(nameof(GetTeamById), new { id = result.Team.Id }, result);
        }

        [HttpGet("myteam")]
        public async Task<IActionResult> GetTeamById()
        {
            var teamId = int.Parse(User.FindFirstValue("teamId")!);
            var result = await _teamService.GetTeamByIdAsync(teamId);
            return Ok(result);
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateTeam(int id, [FromBody] UpdateTeamRequestDto request)
        //{
        //    var result = await _teamService.UpdateTeamAsync(id, request);

        //    if (result == null)
        //    {
        //        return NotFound(new
        //        {
        //            message = $"Team with ID {id} not found"
        //        });
        //    }

        //    return Ok(result);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTeam(int id)
        //{
        //    var success = await _teamService.DeleteTeamAsync(id);

        //    if (!success)
        //    {
        //        return NotFound(new
        //        {
        //            message = $"Team with ID {id} not found"
        //        });
        //    }

        //    return NoContent();
        //}

        [HttpPost("addMembers")]
        public async Task<IActionResult> AddMembers([FromBody] AddTeamMembersRequestDto request)
        {
            var teamId = int.Parse(User.FindFirstValue("teamId")!);
            var result = await _teamService.AddMembersAsync(teamId, request);

            if (result == null || !result.Any())
            {
                return NotFound(
                    new
                    {
                        message = $"Something went wrong trying to add new member with that email. Email might not be registered or already in existing team."
                    }
                );
            }

            return Ok(result);
        }

        [HttpPost("leave")]
        public async Task<IActionResult> LeaveTeam()
        {
            var teamId = int.Parse(User.FindFirstValue("teamId")!);
            var accountId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            var result = await _teamService.RemoveMemberAsync(teamId, accountId);
            if (result == null)
            {
                return NotFound(
                    new
                    {
                        message = $"Something went wrong trying to leave team. Maybe you are not in a team?"
                    }
                );
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
