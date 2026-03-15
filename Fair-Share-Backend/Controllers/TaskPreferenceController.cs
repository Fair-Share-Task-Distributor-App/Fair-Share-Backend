using Fair_Share_Backend.DTOs.TaskPreference;
using Fair_Share_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Fair_Share_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskPreferenceController :ControllerBase
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

        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskPreference(int taskId)
        {
            var accountId = int.Parse(
                User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)!.Value
            );
            var result = await _taskPreferenceService.GetTaskPreferenceAsync(accountId, taskId);

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

        [HttpGet("myPreferences")]
        public async Task<IActionResult> GetTeamTasksPreferencesForAccountId(
            GetTaskPreferenceByTeamDto request
        )
        {
            var userId = int.Parse(
                User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)!.Value
            );
            var result = await _taskPreferenceService.GetTeamTasksPreferencesForAccountIdAsync(
                userId,
                request.TeamId
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
