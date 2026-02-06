using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Settlr.Common.Response;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;

namespace Settlr.Services.IServices;

public interface IGroupService
{
    Task<Response<GroupDto>> CreateGroupAsync(Guid userId, CreateGroupRequestDto request, CancellationToken cancellationToken = default);
    Task<Response<GroupDto>> AddMembersAsync(Guid groupId, AddGroupMembersRequestDto request, CancellationToken cancellationToken = default);
    Task<Response<List<GroupDto>>> GetGroupsAsync(Guid userId, CancellationToken cancellationToken = default);
}
