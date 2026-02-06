using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Settlr.Models.Entities;

namespace Settlr.Data.IRepositories;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Expense>> GetGroupExpensesAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task AddAsync(Expense expense, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
