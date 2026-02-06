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

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GroupService(IGroupRepository groupRepository, IUserRepository userRepository, IMapper mapper)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Response<GroupDto>> CreateGroupAsync(Guid userId, CreateGroupRequestDto request, CancellationToken cancellationToken = default)
    {
        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            CreatedByUserId = userId
        };

        group.Members.Add(new GroupMember
        {
            GroupId = group.Id,
            UserId = userId
        });

        await _groupRepository.AddAsync(group, cancellationToken);
        await _groupRepository.SaveChangesAsync(cancellationToken);

        var created = await _groupRepository.GetByIdWithMembersAsync(group.Id, cancellationToken);
        var dto = _mapper.Map<GroupDto>(created);
        return ResponseFactory.Success(dto, AppMessages.GroupCreated, (int)HttpStatusCode.Created);
    }

    public async Task<Response<GroupDto>> AddMembersAsync(Guid groupId, AddGroupMembersRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.UserIds.Count == 0)
        {
            return ResponseFactory.Fail<GroupDto>(AppMessages.MembersRequired, (int)HttpStatusCode.BadRequest);
        }

        var group = await _groupRepository.GetByIdWithMembersAsync(groupId, cancellationToken);
        if (group == null)
        {
            return ResponseFactory.Fail<GroupDto>(AppMessages.GroupNotFound, (int)HttpStatusCode.NotFound);
        }

        var users = await _userRepository.GetByIdsAsync(request.UserIds, cancellationToken);
        var missingUserIds = request.UserIds.Except(users.Select(x => x.Id)).ToArray();
        if (missingUserIds.Length > 0)
        {
            return ResponseFactory.Fail<GroupDto>(AppMessages.UserNotFound, (int)HttpStatusCode.NotFound, missingUserIds.Select(x => x.ToString()).ToArray());
        }

        await _groupRepository.AddMembersAsync(groupId, request.UserIds, cancellationToken);
        await _groupRepository.SaveChangesAsync(cancellationToken);

        var updated = await _groupRepository.GetByIdWithMembersAsync(groupId, cancellationToken);
        var dto = _mapper.Map<GroupDto>(updated);
        return ResponseFactory.Success(dto, AppMessages.MembersAdded, (int)HttpStatusCode.OK);
    }

    public async Task<Response<List<GroupDto>>> GetGroupsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var groups = await _groupRepository.GetGroupsForUserAsync(userId, cancellationToken);
        var dto = _mapper.Map<List<GroupDto>>(groups);
        return ResponseFactory.Success(dto, AppMessages.GroupList, (int)HttpStatusCode.OK);
    }
}
