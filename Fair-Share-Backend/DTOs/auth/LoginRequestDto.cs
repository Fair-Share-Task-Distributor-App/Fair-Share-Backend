using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fair_Share_Backend.DTOs.Auth
{
    public record LoginRequestDto : IValidatableObject
    {
        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                yield return new ValidationResult("Email must be provided.", [nameof(Email)]);
            }
        }
    }
}
