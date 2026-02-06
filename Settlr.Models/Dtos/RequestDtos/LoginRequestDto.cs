using System.ComponentModel.DataAnnotations;
using Settlr.Common.Helper;

namespace Settlr.Models.Dtos.RequestDtos;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    [MaxLength(ValidationConstants.EmailMaxLength)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(ValidationConstants.PasswordMaxLength, MinimumLength = ValidationConstants.PasswordMinLength)]
    public string Password { get; set; } = null!;
}
