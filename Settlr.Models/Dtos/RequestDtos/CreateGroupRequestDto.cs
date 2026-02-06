using System.ComponentModel.DataAnnotations;
using Settlr.Common.Helper;

namespace Settlr.Models.Dtos.RequestDtos;

public class CreateGroupRequestDto
{
    [Required]
    [MaxLength(ValidationConstants.GroupNameMaxLength)]
    public string Name { get; set; } = null!;
}
