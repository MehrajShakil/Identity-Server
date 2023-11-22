namespace Identity_Server.DTOs;

public class UserResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

