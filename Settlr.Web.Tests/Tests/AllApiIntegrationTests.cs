using System.Net;
using System.Net.Http.Json;
using Settlr.Common.Helper;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Web.Tests.Infrastructure;
using Xunit;

namespace Settlr.Web.Tests.Tests;

public class AllApiIntegrationTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public AllApiIntegrationTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Auth_Register_ReturnsOk()
    {
        var request = new RegisterRequestDto
        {
            Name = "New User",
            Email = "newuser@example.com",
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync(ApiRoutes.AuthRegister, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<AuthResponseDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Auth_Login_ReturnsOk()
    {
        var request = new LoginRequestDto
        {
            Email = "existing@example.com",
            Password = "Password123!"
        };

        var response = await _client.PostAsJsonAsync(ApiRoutes.AuthLogin, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<AuthResponseDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Dashboard_GetSummary_ReturnsOk()
    {
        var response = await _client.GetAsync(ApiRoutes.DashboardSummary);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<UserSummaryDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Groups_CreateGroup_ReturnsOk()
    {
        var request = new CreateGroupRequestDto
        {
            Name = "Test Group"
        };

        var response = await _client.PostAsJsonAsync(ApiRoutes.Groups, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Groups_GetGroups_ReturnsOk()
    {
        var response = await _client.GetAsync(ApiRoutes.Groups);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<List<GroupDto>>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Groups_AddMembers_ReturnsOk()
    {
        // First create a group to get a valid ID
        var groupRequest = new CreateGroupRequestDto { Name = "Member Group" };
        var groupResponse = await _client.PostAsJsonAsync(ApiRoutes.Groups, groupRequest);
        var groupPayload = await groupResponse.Content.ReadFromJsonAsync<Response<GroupDto>>();
        var groupId = groupPayload!.Data!.Id;

        var request = new AddGroupMembersRequestDto
        {
            UserIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        // Construct route with ID
        var route = ApiRoutes.GroupMembers.Replace("{groupId}", groupId.ToString());
        var response = await _client.PostAsJsonAsync(route, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Groups_GetGroupBalances_ReturnsOk()
    {
        // First create a group to get a valid ID
        var groupRequest = new CreateGroupRequestDto { Name = "Balance Group" };
        var groupResponse = await _client.PostAsJsonAsync(ApiRoutes.Groups, groupRequest);
        var groupPayload = await groupResponse.Content.ReadFromJsonAsync<Response<GroupDto>>();
        var groupId = groupPayload!.Data!.Id;

        var route = ApiRoutes.GroupBalances.Replace("{groupId}", groupId.ToString());
        var response = await _client.GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<List<GroupBalanceDto>>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Expenses_CreateExpense_ReturnsOk()
    {
        // Create group first
        var groupRequest = new CreateGroupRequestDto { Name = "Expense Group" };
        var groupResponse = await _client.PostAsJsonAsync(ApiRoutes.Groups, groupRequest);
        var groupPayload = await groupResponse.Content.ReadFromJsonAsync<Response<GroupDto>>();
        var groupId = groupPayload!.Data!.Id;

        var request = new CreateExpenseRequestDto
        {
            GroupId = groupId,
            PayerId = TestAuthHandler.UserId, // Use the test user as payer
            Amount = 50.00m,
            Description = "Lunch"
        };

        var response = await _client.PostAsJsonAsync(ApiRoutes.Expenses, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<ExpenseDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Expenses_GetGroupExpenses_ReturnsOk()
    {
        // Create group first
        var groupRequest = new CreateGroupRequestDto { Name = "History Group" };
        var groupResponse = await _client.PostAsJsonAsync(ApiRoutes.Groups, groupRequest);
        var groupPayload = await groupResponse.Content.ReadFromJsonAsync<Response<GroupDto>>();
        var groupId = groupPayload!.Data!.Id;

        var route = ApiRoutes.GroupExpenses.Replace("{groupId}", groupId.ToString());
        var response = await _client.GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<List<ExpenseDto>>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Users_GetByEmail_ReturnsOk()
    {
        var email = "test@example.com";
        var route = $"{ApiRoutes.UserLookup}?email={email}";
        
        var response = await _client.GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Response<UserDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }
}
