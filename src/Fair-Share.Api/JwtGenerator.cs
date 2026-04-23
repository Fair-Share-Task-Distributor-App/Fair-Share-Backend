using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fair_Share.Api.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Fair_Share.Api
{
    public static class JwtGenerator
    {
        public static string GenerateJwtToken(
            Account account,
            string jwtKey,
            string jwtIssuer,
            string jwtAudience
        )
        {
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256
                );

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                    new Claim("teamId", account.TeamId.ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    }
}
