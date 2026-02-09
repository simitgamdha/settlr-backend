using Microsoft.AspNetCore.Http;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Services.IServices;

namespace Settlr.Web.Tests.TestDoubles;

public class FakeGroupService : IGroupService
{
    public Task<Response<GroupDto>> CreateGroupAsync(Guid userId, CreateGroupRequestDto request, CancellationToken cancellationToken = default)
    {
        var data = BuildGroup(userId, request.Name);
        return Task.FromResult(ResponseFactory.Success(data, "Group created", StatusCodes.Status200OK));
    }

    public Task<Response<GroupDto>> AddMembersAsync(Guid groupId, AddGroupMembersRequestDto request, CancellationToken cancellationToken = default)
    {
        var data = BuildGroup(Guid.NewGuid(), "Test Group");
        return Task.FromResult(ResponseFactory.Success(data, "Members added", StatusCodes.Status200OK));
    }

    public Task<Response<List<GroupDto>>> GetGroupsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var data = new List<GroupDto>
        {
            BuildGroup(userId, "Trip to NYC")
        };

        return Task.FromResult(ResponseFactory.Success(data, "Groups", StatusCodes.Status200OK));
    }

    private static GroupDto BuildGroup(Guid userId, string name)
    {
        return new GroupDto
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }
}
