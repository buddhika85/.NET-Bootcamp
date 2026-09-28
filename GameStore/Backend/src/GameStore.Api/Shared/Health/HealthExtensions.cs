using GameStore.Api.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameStore.Api.Shared.Health;

public static class HealthExtensions
{
    private static readonly string[] AllowedHosts = [
        "*:8081",                   // on prod and everything else if we need to access health endpoints we need to use port 8081, not 8080
        "localhost:5299"            // dev testing can access health endpoints
        ];

    public static void AddGameStoreHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
            .AddDbContextCheck<GameStoreContext>()                                                  // CHECKING APP CAN TALK TO DATABASE via GameStoreContext
            .AddAzureBlobStorage(timeout: TimeSpan.FromSeconds(5));                                 // CHECKING IF AZURE BLOB STORAGE IS RESPONDING
    }


    public static void MapGameStoreHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/ready")
            .RequireHost(AllowedHosts)
            .AllowAnonymous();

        app.MapHealthChecks("/health/alive",
                                        new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                                        {
                                            Predicate = registration => registration.Tags.Contains("live")
                                        })
                                        .RequireHost(AllowedHosts)
                                        .AllowAnonymous();
    }
}
