using System.Threading;
using System.Threading.Tasks;
using Settlr.Common.Response;
using Settlr.Models.Dtos.ResponseDtos;

namespace Settlr.Services.IServices;

public interface IUserService
{
    Task<Response<UserDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
