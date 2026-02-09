using Microsoft.AspNetCore.Http;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;
using Settlr.Web.Tests.Infrastructure;

namespace Settlr.Web.Tests.TestDoubles;

public class FakeAuthService : IAuthService
{
    public Task<Response<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ResponseFactory.Success(BuildAuthResponse(request.Email), "Registered", StatusCodes.Status200OK));
    }

    public Task<Response<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ResponseFactory.Success(BuildAuthResponse(request.Email), "Logged in", StatusCodes.Status200OK));
    }

    private static AuthResponseDto BuildAuthResponse(string email)
    {
        return new AuthResponseDto
        {
            Token = "test-token",
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            User = new UserDto
            {
                Id = TestAuthHandler.UserId,
                Name = "Test User",
                Email = email
            }
        };
    }
}
