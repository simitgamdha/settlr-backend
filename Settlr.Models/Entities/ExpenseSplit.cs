using System;
using System.ComponentModel.DataAnnotations;
using Settlr.Common.Helper;

namespace Settlr.Models.Entities;

public class ExpenseSplit
{
    public Guid ExpenseId { get; set; }
    public Guid UserId { get; set; }
    [Range(typeof(decimal), ValidationConstants.MinAmount, ValidationConstants.MaxAmount)]
    public decimal Amount { get; set; }

    public Expense Expense { get; set; } = null!;
    public AppUser User { get; set; } = null!;
}
