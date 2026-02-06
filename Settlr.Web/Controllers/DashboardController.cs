using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Settlr.Common.Helper;
using Settlr.Common.Messages;
using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;
using Settlr.Web.Extension;

namespace Settlr.Web.Controllers;

[ApiController]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IBalanceService _balanceService;

    public DashboardController(IBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    [HttpGet(ApiRoutes.DashboardSummary)]
    public async Task<ActionResult<Response<UserSummaryDto>>> GetSummary(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(ResponseFactory.Fail<UserSummaryDto>(AppMessages.InvalidCredentials, StatusCodes.Status401Unauthorized));
        }

        var response = await _balanceService.GetUserSummaryAsync(userId.Value, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
