using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Settlr.Services.IServices;
using Settlr.Web.Tests.TestDoubles;

namespace Settlr.Web.Tests.Infrastructure;

public class TestAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAuthService>();
            services.RemoveAll<IGroupService>();
            services.RemoveAll<IExpenseService>();
            services.RemoveAll<IBalanceService>();
            services.RemoveAll<IUserService>();

            services.AddScoped<IAuthService, FakeAuthService>();
            services.AddScoped<IGroupService, FakeGroupService>();
            services.AddScoped<IExpenseService, FakeExpenseService>();
            services.AddScoped<IBalanceService, FakeBalanceService>();
            services.AddScoped<IUserService, FakeUserService>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                    options.DefaultChallengeScheme = TestAuthHandler.Scheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(TestAuthHandler.Scheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });
        });
    }
}
