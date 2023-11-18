using Microsoft.AspNetCore.Identity;

namespace Identity_Server.Entities;

public class ApplicationUser : IdentityUser
{
    public required string UserName { get; set; }
}
