using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Settlr.Common.Helper;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;

namespace Settlr.Web.Controllers;

[ApiController]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpPost(ApiRoutes.Expenses)]
    public async Task<ActionResult<Response<ExpenseDto>>> CreateExpense([FromBody] CreateExpenseRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _expenseService.CreateExpenseAsync(request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet(ApiRoutes.GroupExpenses)]
    public async Task<ActionResult<Response<List<ExpenseDto>>>> GetGroupExpenses([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var response = await _expenseService.GetGroupExpensesAsync(groupId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
