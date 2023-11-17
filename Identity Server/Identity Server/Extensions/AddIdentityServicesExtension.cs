using Identity_Server.Identity_Wrapper_Services;
using Identity_Server.Interfaces;
using Identity_Server.Persistence.Sql_Server;
using Identity_Server.Services;
using Microsoft.AspNetCore.Identity;

namespace Identity_Server.Extensions;

public static class AddIdentityServicesExtension
{
    public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<IdentityUser<int>, IdentityRole<int>>(options =>
        {
            options.User = new UserOptions
            {
                RequireUniqueEmail = true
            };
        })
            .AddUserManager<UserManagerWrapper<IdentityUser<int>>>()
            .AddSignInManager<SigninManagerWrapper<IdentityUser<int>>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        services.AddScoped<IAccountService, AccountService>();
    }
}
