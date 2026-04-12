using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fair_Share.Api.Data;
using Fair_Share.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fair_Share.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly AccountService _accountService;

        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyAccount()
        {
            var accountId = int.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

            var accountData = await _accountService.GetMyAccountAsync(accountId);

            if (accountData == null)
            {
                return NotFound();
            }

            return Ok(accountData);
        }
    }
}
