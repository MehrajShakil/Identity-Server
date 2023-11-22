using Microsoft.AspNetCore.Identity;

namespace Identity_Server.Entities;

public class ApplicationUser : IdentityUser
{
    public required string UserName { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpires { get; set; }


    public bool CanRefreshToken(string refreshToken)
    {
        return string.IsNullOrEmpty(refreshToken)
               && string.Equals(refreshToken, RefreshToken, StringComparison.Ordinal)
               && RefreshTokenExpires <= DateTime.Now;
    }
}
