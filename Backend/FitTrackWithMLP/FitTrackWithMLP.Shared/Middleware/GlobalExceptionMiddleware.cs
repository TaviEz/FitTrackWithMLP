using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FitTrackWithMLP.Shared.Middleware
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                // This logs with the category name of the shared middleware, 
                // but outputs directly to the running service's console/log stream.
                _logger.LogError(ex, "An unhandled exception occurred in the service pipeline.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    Error = "Internal Server Error",
                    Message = "An unexpected error occurred. Please try again later."
                });
            }
        }
    }
}
