using System;

namespace Settlr.Models.Dtos.ResponseDtos;

public class GroupMemberDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime JoinedAt { get; set; }
}
