using Identity_Server.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity_Server.Services;

public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly IConfiguration configuration;

    public JwtTokenProvider(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string GetJwtAccessToken(List<Claim> claims)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                                         configuration["Jwt:Issuer"],
                                         claims,
                                         null,
                                         DateTime.Now.AddMinutes(5),
                                         credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GetJwtRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public List<Claim> GetClaimsFromAccessToken(string token)
    {
        var claims = new List<Claim>();
        return claims;
    }

}
