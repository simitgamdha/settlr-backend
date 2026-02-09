using System.Net;
using System.Net.Http.Json;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Web.Tests.Infrastructure;
using Xunit;

namespace Settlr.Web.Tests.Tests;

public class GroupsControllerTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public GroupsControllerTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateGroup_ReturnsOk()
    {
        CreateGroupRequestDto request = new CreateGroupRequestDto
        {
            Name = "Weekend Trip"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/groups", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<GroupDto>? payload = await response.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task AddMembers_ReturnsOk()
    {
        Guid groupId = Guid.NewGuid();
        AddGroupMembersRequestDto request = new AddGroupMembersRequestDto
        {
            UserIds = new List<Guid> { Guid.NewGuid() }
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync($"/api/groups/{groupId}/members", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<GroupDto>? payload = await response.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task GetGroups_ReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/groups");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<List<GroupDto>>? payload = await response.Content.ReadFromJsonAsync<Response<List<GroupDto>>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task GetGroupBalances_ReturnsOk()
    {
        Guid groupId = Guid.NewGuid();
        HttpResponseMessage response = await _client.GetAsync($"/api/groups/{groupId}/balances");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<List<GroupBalanceDto>>? payload = await response.Content.ReadFromJsonAsync<Response<List<GroupBalanceDto>>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }
}
