using Microsoft.AspNetCore.Http;
using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;

namespace Settlr.Web.Tests.TestDoubles;

public class FakeBalanceService : IBalanceService
{
    public Task<Response<List<GroupBalanceDto>>> GetGroupBalancesAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var data = new List<GroupBalanceDto>
        {
            new GroupBalanceDto
            {
                UserId = Guid.NewGuid(),
                Name = "Member 1",
                OwedByUser = 5.25m,
                OwedToUser = 10.50m,
                NetBalance = 5.25m
            }
        };

        return Task.FromResult(ResponseFactory.Success(data, "Group balances", StatusCodes.Status200OK));
    }

    public Task<Response<UserSummaryDto>> GetUserSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var data = new UserSummaryDto
        {
            TotalOwedByUser = 12.34m,
            TotalOwedToUser = 56.78m
        };

        return Task.FromResult(ResponseFactory.Success(data, "User summary", StatusCodes.Status200OK));
    }
}
