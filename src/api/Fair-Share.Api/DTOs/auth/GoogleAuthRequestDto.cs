using System.ComponentModel.DataAnnotations;

namespace Fair_Share.Api.DTOs.Auth
{
    public record GoogleAuthRequestDto
    {
        [Required]
        public string IdToken { get; set; } = null!;
    }
}
