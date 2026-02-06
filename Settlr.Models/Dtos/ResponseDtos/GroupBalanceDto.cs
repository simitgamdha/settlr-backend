using System;

namespace Settlr.Models.Dtos.ResponseDtos;

public class GroupBalanceDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public decimal OwedByUser { get; set; }
    public decimal OwedToUser { get; set; }
    public decimal NetBalance { get; set; }
}
