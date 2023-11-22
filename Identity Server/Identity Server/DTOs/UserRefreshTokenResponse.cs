namespace Identity_Server.DTOs;

public class UserRefreshTokenResponse : ResponseBase
{
    public string RefreshToken { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpires { get; set; }
}

