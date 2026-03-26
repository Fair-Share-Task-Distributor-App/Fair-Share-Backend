using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fair_Share_Backend.DTOs.Task;
using Fair_Share_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fair_Share_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(TaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequestDto request)
        {
            var teamId = int.Parse(User.FindFirstValue("teamId")!);
            request.TeamId = teamId;

            var result = await _taskService.CreateTaskAsync(request);
            return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var result = await _taskService.GetTaskByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            return Ok(result);
        }

        [HttpGet("unassignedTasks")]
        public async Task<IActionResult> GetUnassignedTasksInTeam()
        {
            var accountId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            var teamId = int.Parse(User.FindFirstValue("teamId")!);

            var results = await _taskService.GetUnassignedTasksInTeamAsync(accountId, teamId);
            return Ok(results);
        }

        [HttpGet("allTasks")]
        public async Task<IActionResult> GetAllTasksInTeam()
        {
            var teamId = int.Parse(User.FindFirstValue("teamId")!);

            var results = await _taskService.GetTasksInTeamAsync(teamId);
            return Ok(results);
        }

        [HttpGet("myTasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
            var results = await _taskService.GetTasksForAccountAsync(userId);
            return Ok(results);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskService.UpdateTaskAsync(id, request);

            if (result == null)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var success = await _taskService.DeleteTaskAsync(id);

            if (!success)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            return NoContent();
        }

        [HttpPost("{id}/assign")]
        public async Task<IActionResult> AssignTask(int id, [FromBody] AssignTaskRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskService.AssignTaskToAccountsAsync(id, request);

            if (result == null)
            {
                return NotFound(new { message = $"Task with ID {id} not found" });
            }

            return Ok(result);
        }
    }
}
