using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Settlr.Common.Helper;
using Settlr.Common.Messages;
using Settlr.Common.Response;
using Settlr.Data.IRepositories;
using Settlr.Models.Dtos.RequestDtos;
using Settlr.Models.Dtos.ResponseDtos;
using Settlr.Models.Entities;
using Settlr.Services.IServices;

namespace Settlr.Services.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly JwtOptions _jwtOptions;
    private readonly PasswordHasher<AppUser> _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IMapper mapper,
        IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _jwtOptions = jwtOptions.Value;
        _passwordHasher = new PasswordHasher<AppUser>();
    }

    public async Task<Response<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var emailExists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailExists)
        {
            return ResponseFactory.Fail<AuthResponseDto>(AppMessages.EmailAlreadyExists, (int)HttpStatusCode.Conflict);
        }

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var response = BuildAuthResponse(user);
        return ResponseFactory.Success(response, AppMessages.RegistrationSuccess, (int)HttpStatusCode.Created);
    }

    public async Task<Response<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return ResponseFactory.Fail<AuthResponseDto>(AppMessages.InvalidCredentials, (int)HttpStatusCode.Unauthorized);
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return ResponseFactory.Fail<AuthResponseDto>(AppMessages.InvalidCredentials, (int)HttpStatusCode.Unauthorized);
        }

        var response = BuildAuthResponse(user);
        return ResponseFactory.Success(response, AppMessages.LoginSuccess, (int)HttpStatusCode.OK);
    }

    private AuthResponseDto BuildAuthResponse(AppUser user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpMinutes);
        var token = CreateToken(user, expiresAt);

        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = _mapper.Map<UserDto>(user)
        };
    }

    private string CreateToken(AppUser user, DateTime expiresAt)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
