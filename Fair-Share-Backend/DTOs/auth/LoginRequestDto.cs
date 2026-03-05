using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fair_Share_Backend.DTOs.Auth
{
    public record LoginRequestDto : IValidatableObject
    {
        public string? Email { get; set; }
        public string? Name { get; set; }

        [Required]
        public string Password { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult(
                    "Either Email or Username must be provided.",
                    [nameof(Email), nameof(Name)]
                );
            }
        }
    }
}
