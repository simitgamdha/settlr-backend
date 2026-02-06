using System;

namespace Settlr.Models.Dtos.ResponseDtos;

public class ExpenseSplitDto
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
}
