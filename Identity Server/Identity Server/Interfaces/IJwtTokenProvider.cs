using System.Security.Claims;

namespace Identity_Server.Interfaces
{
    public interface IJwtTokenProvider
    {
        string GetJwtToken(List<Claim> claims);
    }
}
