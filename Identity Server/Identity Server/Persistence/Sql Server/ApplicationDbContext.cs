using Identity_Server.Entities;
using Identity_Server.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity_Server.Persistence.Sql_Server;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationUserRole, string>
{
	public ApplicationDbContext(DbContextOptions options) : base(options)
	{

	}



    #region Override Methods

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        base.OnConfiguring(builder);
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }

    #endregion




}
