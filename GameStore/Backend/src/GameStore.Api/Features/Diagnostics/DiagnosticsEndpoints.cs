using System;
using GameStore.Api.Features.Diagnostics.GetNodeInfo;

namespace GameStore.Api.Features.Diagnostics;

public static class DiagnosticsEndpoints
{
    public static void MapDiagnostics(this IEndpointRouteBuilder app)
    {
        // Apply the common prefix for all genres endpoint
        var group = app.MapGroup("/diagnostics");

        // GET /NodeInfo/
        group.MapGetNodeInfo();
    }
}
