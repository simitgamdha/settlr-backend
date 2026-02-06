using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Settlr.Common.Helper;

namespace Settlr.Models.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(ValidationConstants.NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(ValidationConstants.EmailMaxLength)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(ValidationConstants.PasswordMaxLength)]
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<GroupMember> GroupMemberships { get; set; } = new List<GroupMember>();
    public ICollection<Group> GroupsCreated { get; set; } = new List<Group>();
    public ICollection<Expense> ExpensesPaid { get; set; } = new List<Expense>();
    public ICollection<ExpenseSplit> ExpenseSplits { get; set; } = new List<ExpenseSplit>();
}
