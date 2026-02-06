using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Settlr.Common.Helper;
using Settlr.Common.Messages;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;
using Settlr.Web.Extension;

namespace Settlr.Web.Controllers;

[ApiController]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;
    private readonly IBalanceService _balanceService;

    public GroupsController(IGroupService groupService, IBalanceService balanceService)
    {
        _groupService = groupService;
        _balanceService = balanceService;
    }

    [HttpPost(ApiRoutes.Groups)]
    public async Task<ActionResult<Response<GroupDto>>> CreateGroup([FromBody] CreateGroupRequestDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(ResponseFactory.Fail<GroupDto>(AppMessages.InvalidCredentials, StatusCodes.Status401Unauthorized));
        }

        var response = await _groupService.CreateGroupAsync(userId.Value, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost(ApiRoutes.GroupMembers)]
    public async Task<ActionResult<Response<GroupDto>>> AddMembers([FromRoute] Guid groupId, [FromBody] AddGroupMembersRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _groupService.AddMembersAsync(groupId, request, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet(ApiRoutes.Groups)]
    public async Task<ActionResult<Response<List<GroupDto>>>> GetGroups(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        if (userId == null)
        {
            return Unauthorized(ResponseFactory.Fail<List<GroupDto>>(AppMessages.InvalidCredentials, StatusCodes.Status401Unauthorized));
        }

        var response = await _groupService.GetGroupsAsync(userId.Value, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet(ApiRoutes.GroupBalances)]
    public async Task<ActionResult<Response<List<GroupBalanceDto>>>> GetGroupBalances([FromRoute] Guid groupId, CancellationToken cancellationToken)
    {
        var response = await _balanceService.GetGroupBalancesAsync(groupId, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
