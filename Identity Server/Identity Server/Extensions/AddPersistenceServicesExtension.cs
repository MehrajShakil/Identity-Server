using Identity_Server.Persistence.Sql_Server;
using Microsoft.EntityFrameworkCore;

namespace Identity_Server.Extensions;

public static class AddPersistenceServicesExtension
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(dbContextOptionsBuilder =>
        {
            dbContextOptionsBuilder.UseSqlServer(configuration.GetConnectionString("SqlServer"));
        });
    }
}
