using Microsoft.AspNetCore.Http;
using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;
using Settlr.Web.Tests.Infrastructure;

namespace Settlr.Web.Tests.TestDoubles;

public class FakeUserService : IUserService
{
    public Task<Response<UserDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var data = new UserDto
        {
            Id = TestAuthHandler.UserId,
            Name = "Test User",
            Email = email
        };

        return Task.FromResult(ResponseFactory.Success(data, "User found", StatusCodes.Status200OK));
    }
}
