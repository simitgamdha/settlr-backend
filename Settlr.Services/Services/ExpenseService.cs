using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Settlr.Common.Messages;
using Settlr.Common.Response;
using Settlr.Data.IRepositories;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Models.Entities;
using Settlr.Services.IServices;

namespace Settlr.Services.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IMapper _mapper;

    public ExpenseService(IExpenseRepository expenseRepository, IGroupRepository groupRepository, IMapper mapper)
    {
        _expenseRepository = expenseRepository;
        _groupRepository = groupRepository;
        _mapper = mapper;
    }

    public async Task<Response<ExpenseDto>> CreateExpenseAsync(CreateExpenseRequestDto request, CancellationToken cancellationToken = default)
    {
        Group? group = await _groupRepository.GetByIdWithMembersAsync(request.GroupId, cancellationToken);
        if (group == null)
        {
            return ResponseFactory.Fail<ExpenseDto>(AppMessages.GroupNotFound, (int)HttpStatusCode.NotFound);
        }

        bool payerInGroup = group.Members.Any(x => x.UserId == request.PayerId);
        if (!payerInGroup)
        {
            return ResponseFactory.Fail<ExpenseDto>(AppMessages.PayerNotInGroup, (int)HttpStatusCode.BadRequest);
        }

        List<Guid> memberIds = group.Members.Select(x => x.UserId).ToList();
        List<ExpenseSplit> splits = BuildEqualSplits(memberIds, request.Amount);

        Expense expense = new Expense
        {
            Id = Guid.NewGuid(),
            GroupId = request.GroupId,
            PayerId = request.PayerId,
            Amount = request.Amount,
            Description = request.Description,
            Splits = splits
        };

        await _expenseRepository.AddAsync(expense, cancellationToken);
        await _expenseRepository.SaveChangesAsync(cancellationToken);

        ExpenseDto dto = _mapper.Map<ExpenseDto>(expense);
        return ResponseFactory.Success(dto, AppMessages.ExpenseCreated, (int)HttpStatusCode.Created);
    }

    public async Task<Response<List<ExpenseDto>>> GetGroupExpensesAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Expense> expenses = await _expenseRepository.GetGroupExpensesAsync(groupId, cancellationToken);
        List<ExpenseDto> dto = _mapper.Map<List<ExpenseDto>>(expenses);
        return ResponseFactory.Success(dto, AppMessages.ExpenseList, (int)HttpStatusCode.OK);
    }

    private static List<ExpenseSplit> BuildEqualSplits(List<Guid> memberIds, decimal amount)
    {
        int count = memberIds.Count;
        List<ExpenseSplit> splits = new List<ExpenseSplit>(count);
        if (count == 0)
        {
            return splits;
        }

        decimal share = Math.Round(amount / count, 2, MidpointRounding.AwayFromZero);
        decimal remainder = amount - (share * (count - 1));

        for (int i = 0; i < count; i++)
        {
            splits.Add(new ExpenseSplit
            {
                UserId = memberIds[i],
                Amount = i == count - 1 ? remainder : share
            });
        }

        return splits;
    }
}
