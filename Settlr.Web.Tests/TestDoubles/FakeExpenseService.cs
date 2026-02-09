using Microsoft.AspNetCore.Http;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;

namespace Settlr.Web.Tests.TestDoubles;

public class FakeExpenseService : IExpenseService
{
    public Task<Response<ExpenseDto>> CreateExpenseAsync(CreateExpenseRequestDto request, CancellationToken cancellationToken = default)
    {
        var data = new ExpenseDto
        {
            Id = Guid.NewGuid(),
            GroupId = request.GroupId,
            PayerId = request.PayerId,
            Amount = request.Amount,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        return Task.FromResult(ResponseFactory.Success(data, "Expense created", StatusCodes.Status200OK));
    }

    public Task<Response<List<ExpenseDto>>> GetGroupExpensesAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var data = new List<ExpenseDto>
        {
            new ExpenseDto
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                PayerId = Guid.NewGuid(),
                Amount = 25.00m,
                Description = "Lunch",
                CreatedAt = DateTime.UtcNow
            }
        };

        return Task.FromResult(ResponseFactory.Success(data, "Group expenses", StatusCodes.Status200OK));
    }
}
