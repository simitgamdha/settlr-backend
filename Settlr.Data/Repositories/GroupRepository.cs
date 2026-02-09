using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Settlr.Data.DbContext;
using Settlr.Data.IRepositories;
using Settlr.Models.Entities;

namespace Settlr.Data.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly ApplicationDbContext _context;

    public GroupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Group?> GetByIdWithMembersAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return _context.Groups
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);
    }

    public async Task<IReadOnlyList<Group>> GetGroupsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(x => x.Members)
            .ThenInclude(x => x.User)
            .Where(x => x.Members.Any(m => m.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsUserInGroupAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.GroupMembers.AnyAsync(x => x.GroupId == groupId && x.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        await _context.Groups.AddAsync(group, cancellationToken);
    }

    public async Task AddMembersAsync(Guid groupId, IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
        List<Guid> existingMemberIds = await _context.GroupMembers
            .Where(x => x.GroupId == groupId && userIds.Contains(x.UserId))
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        IEnumerable<GroupMember> newMembers = userIds
            .Where(id => !existingMemberIds.Contains(id))
            .Select(id => new GroupMember
            {
                GroupId = groupId,
                UserId = id
            });

        await _context.GroupMembers.AddRangeAsync(newMembers, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
