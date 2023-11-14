using Microsoft.AspNetCore.Mvc;

namespace Identity_Server.Helpers
{
    public static class ActionResutlHelper
    {
        public static IActionResult ReturnActionResult<TResponse>(TResponse response, string statusCode) where TResponse : class
        {
            switch (statusCode)
            {
                case "401":
                    return new UnauthorizedObjectResult(response);
                case "409":
                    return new ConflictObjectResult(response);
                default:
                    return new OkObjectResult(response);
            }
        }
    }
}
