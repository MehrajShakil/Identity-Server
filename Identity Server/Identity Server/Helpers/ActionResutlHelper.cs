using Microsoft.AspNetCore.Mvc;

namespace Identity_Server.Helpers
{
    public static class ActionResutlHelper
    {
        public static IActionResult ReturnActionResult<TResponse>(TResponse response, int statusCode) where TResponse : class
        {
            switch (statusCode)
            {
                case 401:
                    return new UnauthorizedObjectResult(response);
                case 409:
                    return new ConflictObjectResult(response);
                case 502:
                    return new ObjectResult(response) { StatusCode = statusCode };
                default:
                    return new OkObjectResult(response);
            }
        }
    }
}
