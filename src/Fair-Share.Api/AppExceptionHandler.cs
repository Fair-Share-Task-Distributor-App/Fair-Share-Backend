using System.Data;
using Microsoft.AspNetCore.Diagnostics;
using static Google.Apis.Requests.BatchRequest;

namespace Fair_Share.Api
{
    public class AppExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<AppExceptionHandler> _logger;

        public AppExceptionHandler(ILogger<AppExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            _logger.LogError(
                exception,
                "An unhandled exception occurred while processing the request."
            );
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(
                new { message = "An unexpected error occurred." }
            );

            return true;
        }
    }
}
