using Identity_Server.Entities;
using Identity_Server.Identity_Wrapper_Services;
using Identity_Server.Persistence.Sql_Server;
using Microsoft.AspNetCore.Identity;

namespace Identity_Server.Extensions;

public static class AddIdentityServicesExtension
{
    public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, ApplicationUserRole>(options =>
        {
            options.User = new UserOptions
            {
                RequireUniqueEmail = true
            };
        })
            .AddUserManager<UserManagerWrapper>()
            .AddSignInManager<SigninManagerWrapper>()
            .AddRoleManager<RoleManagerWrapper<ApplicationUserRole>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
}
