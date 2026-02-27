using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fair_Share_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Ok(new { message = "Work in progress" });
    }

    [HttpGet("privacy")]
    public IActionResult Privacy()
    {
        return Ok(new { message = "Privacy endpoint" });
    }

    [HttpGet("error")]
    public IActionResult Error()
    {
        return Problem(detail: "An unexpected error occurred.", statusCode: 500);
    }
}
