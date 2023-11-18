namespace Identity_Server.DTOs;

public class ResponseBase
{
    public string Id { get; set; } = string.Empty;
    public List<string> Messages { get; set; } = new();
    public int StatusCode { get; set; }
}
