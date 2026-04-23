using Fair_Share.Api.DTOs.Auth;
using Fair_Share.Api.Entities;

namespace Fair_Share.Api.Mappers
{
    public class AccountMapper
    {
        public Account ToEntity(SignupRequestDto request, string passwordHash)
        {
            return new Account
            {
                Email = request.Email,
                Name = request.Name,
                PasswordHash = passwordHash
            };
        }

        public AuthResponseDto ToAuthResponseDto(Account account, string token, bool isNewUser)
        {
            return new AuthResponseDto
            {
                Token = token,
                Email = account.Email,
                Name = account.Name,
                IsNewUser = isNewUser,
                teamName = account.Team != null ? account.Team.Name : null
            };
        }
    }
}
