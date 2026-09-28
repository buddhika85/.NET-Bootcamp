using System;

namespace GameStore.Api.Features.Diagnostics.GetNodeInfo;

public static class GetNodeInfoEndpoint
{
    public static void MapGetNodeInfo(this IEndpointRouteBuilder app)
    {
        app.MapGet("/NodeInfo", () =>
        {
            return new { NodeName = Environment.MachineName };
        });
    }
}