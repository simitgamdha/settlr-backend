using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Settlr.Common.Helper;

namespace Settlr.Models.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid PayerId { get; set; }
    [Range(typeof(decimal), ValidationConstants.MinAmount, ValidationConstants.MaxAmount)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(ValidationConstants.ExpenseDescriptionMaxLength)]
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Group Group { get; set; } = null!;
    public AppUser Payer { get; set; } = null!;
    public ICollection<ExpenseSplit> Splits { get; set; } = new List<ExpenseSplit>();
}
