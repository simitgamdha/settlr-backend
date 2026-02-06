using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;

namespace Settlr.Services.IServices;

public interface IExpenseService
{
    Task<Response<ExpenseDto>> CreateExpenseAsync(CreateExpenseRequestDto request, CancellationToken cancellationToken = default);
    Task<Response<List<ExpenseDto>>> GetGroupExpensesAsync(Guid groupId, CancellationToken cancellationToken = default);
}
