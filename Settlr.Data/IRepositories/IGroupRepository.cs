using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Settlr.Models.Entities;

namespace Settlr.Data.IRepositories;

public interface IGroupRepository
{
    Task<Group?> GetByIdWithMembersAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Group>> GetGroupsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserInGroupAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Group group, CancellationToken cancellationToken = default);
    Task AddMembersAsync(Guid groupId, IEnumerable<Guid> userIds, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
