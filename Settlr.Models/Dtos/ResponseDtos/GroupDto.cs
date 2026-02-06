using System;
using System.Collections.Generic;

namespace Settlr.Models.Dtos.ResponseDtos;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<GroupMemberDto> Members { get; set; } = new();
}
