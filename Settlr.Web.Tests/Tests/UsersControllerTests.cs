using System.Net;
using System.Net.Http.Json;
using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Web.Tests.Infrastructure;
using Xunit;

namespace Settlr.Web.Tests.Tests;

public class UsersControllerTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public UsersControllerTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetByEmail_WithEmail_ReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/users/lookup?email=test@example.com");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<UserDto>? payload = await response.Content.ReadFromJsonAsync<Response<UserDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task GetByEmail_WithoutEmail_ReturnsBadRequest()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/users/lookup");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
