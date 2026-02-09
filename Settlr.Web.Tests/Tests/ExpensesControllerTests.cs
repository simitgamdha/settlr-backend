using System.Net;
using System.Net.Http.Json;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Web.Tests.Infrastructure;
using Xunit;

namespace Settlr.Web.Tests.Tests;

public class ExpensesControllerTests : IClassFixture<TestAppFactory>
{
    private readonly HttpClient _client;

    public ExpensesControllerTests(TestAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateExpense_ReturnsOk()
    {
        CreateExpenseRequestDto request = new CreateExpenseRequestDto
        {
            GroupId = Guid.NewGuid(),
            PayerId = Guid.NewGuid(),
            Amount = 20.50m,
            Description = "Dinner"
        };

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/expenses", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<ExpenseDto>? payload = await response.Content.ReadFromJsonAsync<Response<ExpenseDto>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }

    [Fact]
    public async Task GetGroupExpenses_ReturnsOk()
    {
        Guid groupId = Guid.NewGuid();
        HttpResponseMessage response = await _client.GetAsync($"/api/groups/{groupId}/expenses");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Response<List<ExpenseDto>>? payload = await response.Content.ReadFromJsonAsync<Response<List<ExpenseDto>>>();
        Assert.NotNull(payload);
        Assert.True(payload!.Succeeded);
    }
}
