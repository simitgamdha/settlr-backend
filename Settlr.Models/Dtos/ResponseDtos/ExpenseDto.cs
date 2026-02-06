using System;
using System.Collections.Generic;

namespace Settlr.Models.Dtos.ResponseDtos;

public class ExpenseDto
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid PayerId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<ExpenseSplitDto> Splits { get; set; } = new();
}
