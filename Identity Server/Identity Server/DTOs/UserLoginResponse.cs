﻿using System.Security.Claims;

namespace Identity_Server.DTOs;

public sealed class UserLoginResponse : ResponseBase
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public List<Claim> Claims { get; set; } = new();
}
