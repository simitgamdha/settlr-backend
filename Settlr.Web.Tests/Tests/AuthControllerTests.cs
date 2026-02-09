using System.Net;
using System.Net.Http.Json;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Web.Tests.Infrastructure;
using Xunit;

namespace Settlr.Web.Tests.Tests;

public class AuthControllerTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public AuthControllerTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsOk()
    {
        RegisterRequestDto request = new RegisterRequestDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123!"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<AuthResponseDto>? payload = await response.Content.ReadFromJsonAsync<Response<AuthResponseDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Register_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = new
        {
            Name = "Test User",
            Password = "Password123!"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidRequest_ReturnsOk()
    {
        LoginRequestDto request = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<AuthResponseDto>? payload = await response.Content.ReadFromJsonAsync<Response<AuthResponseDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Login_WithInvalidRequest_ReturnsBadRequest()
    {
        var request = new
        {
            Password = "Password123!"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/auth/login", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
