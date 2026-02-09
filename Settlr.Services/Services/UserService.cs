using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Settlr.Common.Messages;
using Settlr.Common.Response;
using Settlr.Data.IRepositories;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Models.Entities;
using Settlr.Services.IServices;

namespace Settlr.Services.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<Response<UserDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        AppUser? user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user == null)
        {
            return ResponseFactory.Fail<UserDto>(AppMessages.UserNotFound, (int)HttpStatusCode.NotFound);
        }

        UserDto dto = _mapper.Map<UserDto>(user);
        return ResponseFactory.Success(dto, AppMessages.UserFound, (int)HttpStatusCode.OK);
    }
}
