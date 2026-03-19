using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fair_Share_Backend.DTOs.TaskPreference;
using Fair_Share_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fair_Share_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskPreferenceController : ControllerBase
    {
        private readonly TaskPreferenceService _taskPreferenceService;
        private readonly ILogger<TaskPreferenceController> _logger;

        public TaskPreferenceController(
            TaskPreferenceService taskPreferenceService,
            ILogger<TaskPreferenceController> logger
        )
        {
            _taskPreferenceService = taskPreferenceService;
            _logger = logger;
        }

        [HttpGet("myPreferences")]
        public async Task<IActionResult> GetTeamTasksPreferencesForAccountId()
        {
            var userId = int.Parse(
                User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)!.Value
            );
            var teamId = int.Parse(User.FindFirstValue("teamId")!);

            var result = await _taskPreferenceService.GetTeamTasksPreferencesForAccountIdAsync(
                userId,
                teamId
            );

            return Ok(result);
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTaskPreference(
            int taskId,
            [FromBody] UpdateTaskPreferenceRequestDto request
        )
        {
            var accountId = int.Parse(
                User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)!.Value
            );

            var result = await _taskPreferenceService.UpdateTaskPreferenceAsync(
                accountId,
                taskId,
                request
            );

            if (result == null)
            {
                return NotFound(
                    new
                    {
                        message = $"Task preference not found for AccountId {accountId} and TaskId {taskId}"
                    }
                );
            }

            return Ok(result);
        }
    }
}
