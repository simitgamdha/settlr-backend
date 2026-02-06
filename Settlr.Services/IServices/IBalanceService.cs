using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;

namespace Settlr.Services.IServices;

public interface IBalanceService
{
    Task<Response<List<GroupBalanceDto>>> GetGroupBalancesAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<Response<UserSummaryDto>> GetUserSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
}
