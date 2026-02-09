using System.Net;
using System.Net.Http.Json;
using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Web.Tests.Infrastructure;
using Xunit;

namespace Settlr.Web.Tests.Tests;

public class DashboardControllerTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public DashboardControllerTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSummary_ReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/dashboard/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<UserSummaryDto>? payload = await response.Content.ReadFromJsonAsync<Response<UserSummaryDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }
}
