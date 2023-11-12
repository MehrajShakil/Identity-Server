namespace Identity_Server.DTOs
{
    public class ResponseBase
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
    }
}
