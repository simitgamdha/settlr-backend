using System;
using System.ComponentModel.DataAnnotations;
using Settlr.Common.Helper;

namespace Settlr.Models.Dtos.RequestDtos;

public class CreateExpenseRequestDto
{
    [Required]
    public Guid GroupId { get; set; }

    [Required]
    public Guid PayerId { get; set; }

    [Range(typeof(decimal), ValidationConstants.MinAmount, ValidationConstants.MaxAmount)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(ValidationConstants.ExpenseDescriptionMaxLength)]
    public string Description { get; set; } = null!;
}
