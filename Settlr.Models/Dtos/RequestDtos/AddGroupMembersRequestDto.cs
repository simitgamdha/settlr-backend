using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Settlr.Models.Dtos.RequestDtos;

public class AddGroupMembersRequestDto
{
    [Required]
    [MinLength(1)]
    public List<Guid> UserIds { get; set; } = new();
}
