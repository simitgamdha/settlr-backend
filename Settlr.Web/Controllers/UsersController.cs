using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Settlr.Common.Helper;
using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;

namespace Settlr.Web.Controllers;

[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet(ApiRoutes.UserLookup)]
    public async Task<ActionResult<Response<UserDto>>> GetByEmail([FromQuery] string email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            Response<UserDto> errorResponse = ResponseFactory.Fail<UserDto>("Email is required.", StatusCodes.Status400BadRequest);
            return StatusCode(errorResponse.StatusCode, errorResponse);
        }

        Response<UserDto> response = await _userService.GetByEmailAsync(email, cancellationToken);
        return StatusCode(response.StatusCode, response);
    }
}
