using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace GameStore.Api.Shared.ErrorHandling;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.TraceId;
        logger.LogError(
            exception,
            "Could not process a request on Machine {Machine}. TraceId: {TraceId}",
            Environment.MachineName,
            traceId);

        await TypedResults.Problem(
            title: "An error occured while processing your request",
            statusCode: StatusCodes.Status500InternalServerError,
            extensions: new Dictionary<string, object?>
            {
                        {"traceId",  traceId.ToString()}
            }).ExecuteAsync(httpContext);

        return true;            // exception is handled successfuly, nothing left to do.
    }
}
