using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Settlr.Common.Helper;

namespace Settlr.Models.Entities;

public class Group
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(ValidationConstants.GroupNameMaxLength)]
    public string Name { get; set; } = null!;
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser CreatedByUser { get; set; } = null!;
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
