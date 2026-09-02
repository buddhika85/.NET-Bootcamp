using System.Diagnostics;

namespace GameStore.Api.Shared.Timing;

public class RequestTimingMiddleware(
    RequestDelegate next,
    ILogger<RequestTimingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopWatch = new Stopwatch();
        try
        {
            stopWatch.Start();
            await next(context);        // call next middleware
        }
        finally
        {
            stopWatch.Stop();
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation(
                    "\n------->{RequestMethod} {RequestPath} completed with status code {Status} in {ElapsedMilliseconds} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopWatch.ElapsedMilliseconds);
        }
    }
}
