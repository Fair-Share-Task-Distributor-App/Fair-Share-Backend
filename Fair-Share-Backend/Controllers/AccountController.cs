//using Microsoft.AspNetCore.Mvc;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

//namespace Fair_Share_Backend.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AccountController :Controller
//    {
//        [HttpGet("me")]
//        public async Task<IActionResult> GetMyAccount()
//        {
//            var accountId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

//            // Need to return name, email, password, points, and number of tasks completed. Do they refresh some day? wont it get very high later? In db, do I delete completed tasks? or idk
//        }

//    }
