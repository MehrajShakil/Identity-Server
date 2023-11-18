using System.Security.Claims;

namespace Identity_Server.Interfaces
{
    public interface IJwtTokenProvider
    {
        List<Claim> GetClaimsFromAccessToken(string token);
        string GetJwtAccessToken(List<Claim> claims);
        string GetJwtRefreshToken();
    }
}
