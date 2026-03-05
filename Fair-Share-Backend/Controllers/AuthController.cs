using Fair_Share_Backend.DTOs.Auth;
using Fair_Share_Backend.Mappers;
using Fair_Share_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fair_Share_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequestDto request)
        {
            var result = await _authService.AuthenticateWithGoogleAsync(request.IdToken);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid Google token" });
            }

            _logger.LogInformation("User authenticated with Google: {Email}", result.Email);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(result);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupRequestDto request)
        {
            var result = await _authService.SignupAsync(request);

            if (result == null)
            {
                return Conflict(new { message = "User with this email already exists" });
            }

            return CreatedAtAction(nameof(Signup), new { id = result.AccountId }, result);
        }
    }
}
