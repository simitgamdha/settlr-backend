using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Settlr.Common.Helper;
using Settlr.Common.Messages;
using Settlr.Common.Response;
using Settlr.Data.DbContext;
using Settlr.Data.IRepositories;
using Settlr.Data.Repositories;
using Settlr.Services.IServices;
using Settlr.Services.Services;
using Settlr.Web.Mappings;

namespace Settlr.Web.Extension;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSettlrControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    string[] errors = context.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();

                    Response<object> response = ResponseFactory.Fail<object>(AppMessages.ValidationFailed, StatusCodes.Status400BadRequest, errors);
                    return new BadRequestObjectResult(response);
                };
            });

        return services;
    }

    public static IServiceCollection AddSettlrDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(ConfigKeys.DefaultConnection)));

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();

        return services;
    }

    public static IServiceCollection AddSettlrServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IBalanceService, BalanceService>();
        services.AddScoped<IUserService, UserService>();

        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        return services;
    }

    public static IServiceCollection AddSettlrAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(ConfigKeys.JwtSection));
        JwtOptions jwtOptions = configuration.GetSection(ConfigKeys.JwtSection).Get<JwtOptions>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddSettlrSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(SwaggerDefaults.ApiVersion, new OpenApiInfo
            {
                Title = SwaggerDefaults.ApiTitle,
                Version = SwaggerDefaults.ApiVersion
            });

            OpenApiSecurityScheme securityScheme = new OpenApiSecurityScheme
            {
                Name = HeaderNames.Authorization,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header,
                Description = JwtBearerDefaults.AuthenticationScheme
            };

            options.AddSecurityDefinition(SwaggerDefaults.SecuritySchemeId, securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = SwaggerDefaults.SecuritySchemeId
                        }
                    },
                    new string[] { }
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddSettlrCors(this IServiceCollection services, IConfiguration configuration)
    {
        string[] origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("SettlrCors", policy =>
            {
                if (origins.Length > 0)
                {
                    policy.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
                else
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(_ => true);
                }
            });
        });

        return services;
    }
}
