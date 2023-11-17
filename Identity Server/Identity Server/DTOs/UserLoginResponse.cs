namespace Identity_Server.DTOs;

public sealed class UserLoginResponse : ResponseBase
{
    public string AccessKey { get; set; }
    public string RefreshKey { get; set; }
}
