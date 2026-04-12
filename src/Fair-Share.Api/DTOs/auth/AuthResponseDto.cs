using System.ComponentModel.DataAnnotations;

namespace Fair_Share.Api.DTOs.Auth
{
    public record AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public bool IsNewUser { get; set; }
        public string? teamName { get; set; }
    }
}
