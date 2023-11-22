namespace Identity_Server.DTOs;

public class UserRefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

