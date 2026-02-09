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
        RegisterRequestDto request = new RegisterRequestDto
        {
            Name = "New User",
            Email = "newuser@example.com",
            Password = "Password123!"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(ApiRoutes.AuthRegister, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<AuthResponseDto>? payload = await response.Content.ReadFromJsonAsync<Response<AuthResponseDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Auth_Login_ReturnsOk()
    {
        LoginRequestDto request = new LoginRequestDto
        {
            Email = "existing@example.com",
            Password = "Password123!"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(ApiRoutes.AuthLogin, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<AuthResponseDto>? payload = await response.Content.ReadFromJsonAsync<Response<AuthResponseDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Dashboard_GetSummary_ReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync(ApiRoutes.DashboardSummary);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<UserSummaryDto>? payload = await response.Content.ReadFromJsonAsync<Response<UserSummaryDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Groups_CreateGroup_ReturnsOk()
    {
        CreateGroupRequestDto request = new CreateGroupRequestDto
        {
            Name = "Test Group"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(ApiRoutes.Groups, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<GroupDto>? payload = await response.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Groups_GetGroups_ReturnsOk()
    {
        HttpResponseMessage response = await _client.GetAsync(ApiRoutes.Groups);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<List<GroupDto>>? payload = await response.Content.ReadFromJsonAsync<Response<List<GroupDto>>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Groups_AddMembers_ReturnsOk()
    {
        // First create a group to get a valid ID
        CreateGroupRequestDto groupRequest = new CreateGroupRequestDto { Name = "Member Group" };
        HttpResponseMessage groupResponse = await _client.PostAsJsonAsync(ApiRoutes.Groups, groupRequest);
        Response<GroupDto>? groupPayload = await groupResponse.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Guid groupId = groupPayload!.Data!.Id;

        AddGroupMembersRequestDto request = new AddGroupMembersRequestDto
        {
            UserIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        // Construct route with ID
        string route = ApiRoutes.GroupMembers.Replace("{groupId}", groupId.ToString());
        HttpResponseMessage response = await _client.PostAsJsonAsync(route, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<GroupDto>? payload = await response.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Groups_GetGroupBalances_ReturnsOk()
    {
        // First create a group to get a valid ID
        CreateGroupRequestDto groupRequest = new CreateGroupRequestDto { Name = "Balance Group" };
        HttpResponseMessage groupResponse = await _client.PostAsJsonAsync(ApiRoutes.Groups, groupRequest);
        Response<GroupDto>? groupPayload = await groupResponse.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Guid groupId = groupPayload!.Data!.Id;

        string route = ApiRoutes.GroupBalances.Replace("{groupId}", groupId.ToString());
        HttpResponseMessage response = await _client.GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<List<GroupBalanceDto>>? payload = await response.Content.ReadFromJsonAsync<Response<List<GroupBalanceDto>>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Expenses_CreateExpense_ReturnsOk()
    {
        // Create group first
        CreateGroupRequestDto groupRequest = new CreateGroupRequestDto { Name = "Expense Group" };
        HttpResponseMessage groupResponse = await _client.PostAsJsonAsync(ApiRoutes.Groups, groupRequest);
        Response<GroupDto>? groupPayload = await groupResponse.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Guid groupId = groupPayload!.Data!.Id;

        CreateExpenseRequestDto request = new CreateExpenseRequestDto
        {
            GroupId = groupId,
            PayerId = TestAuthHandler.UserId, // Use the test user as payer
            Amount = 50.00m,
            Description = "Lunch"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync(ApiRoutes.Expenses, request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<ExpenseDto>? payload = await response.Content.ReadFromJsonAsync<Response<ExpenseDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Expenses_GetGroupExpenses_ReturnsOk()
    {
        // Create group first
        CreateGroupRequestDto groupRequest = new CreateGroupRequestDto { Name = "History Group" };
        HttpResponseMessage groupResponse = await _client.PostAsJsonAsync(ApiRoutes.Groups, groupRequest);
        Response<GroupDto>? groupPayload = await groupResponse.Content.ReadFromJsonAsync<Response<GroupDto>>();
        Guid groupId = groupPayload!.Data!.Id;

        string route = ApiRoutes.GroupExpenses.Replace("{groupId}", groupId.ToString());
        HttpResponseMessage response = await _client.GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<List<ExpenseDto>>? payload = await response.Content.ReadFromJsonAsync<Response<List<ExpenseDto>>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task Users_GetByEmail_ReturnsOk()
    {
        string email = "test@example.com";
        string route = $"{ApiRoutes.UserLookup}?email={email}";
        
        HttpResponseMessage response = await _client.GetAsync(route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<UserDto>? payload = await response.Content.ReadFromJsonAsync<Response<UserDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }
}
