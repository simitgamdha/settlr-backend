using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Settlr.Common.Messages;
using Settlr.Common.Response;
using Settlr.Data.DbContext;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;

namespace Settlr.Services.Services;

public class BalanceService : IBalanceService
{
    private readonly ApplicationDbContext _context;

    public BalanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Response<List<GroupBalanceDto>>> GetGroupBalancesAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var group = await _context.Groups
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .Include(x => x.Expenses)
            .ThenInclude(x => x.Splits)
            .FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);

        if (group == null)
        {
            return ResponseFactory.Fail<List<GroupBalanceDto>>(AppMessages.GroupNotFound, (int)HttpStatusCode.NotFound);
        }

        var balances = new List<GroupBalanceDto>();
        foreach (var member in group.Members)
        {
            var totalPaid = group.Expenses.Where(x => x.PayerId == member.UserId).Sum(x => x.Amount);
            var totalShare = group.Expenses.SelectMany(x => x.Splits).Where(x => x.UserId == member.UserId).Sum(x => x.Amount);
            var net = Math.Round(totalPaid - totalShare, 2, MidpointRounding.AwayFromZero);

            balances.Add(new GroupBalanceDto
            {
                UserId = member.UserId,
                Name = member.User.Name,
                OwedToUser = net > 0 ? net : 0,
                OwedByUser = net < 0 ? Math.Abs(net) : 0,
                NetBalance = net
            });
        }

        return ResponseFactory.Success(balances, AppMessages.GroupBalance, (int)HttpStatusCode.OK);
    }

    public async Task<Response<UserSummaryDto>> GetUserSummaryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var totalPaid = await _context.Expenses.Where(x => x.PayerId == userId).SumAsync(x => x.Amount, cancellationToken);
        var totalShare = await _context.ExpenseSplits.Where(x => x.UserId == userId).SumAsync(x => x.Amount, cancellationToken);
        var net = Math.Round(totalPaid - totalShare, 2, MidpointRounding.AwayFromZero);

        var summary = new UserSummaryDto
        {
            TotalOwedToUser = net > 0 ? net : 0,
            TotalOwedByUser = net < 0 ? Math.Abs(net) : 0
        };

        return ResponseFactory.Success(summary, AppMessages.Summary, (int)HttpStatusCode.OK);
    }
}
