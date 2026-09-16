using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GameStore.Api.Shared.Authorization;

public class EntraClaimsTransformer(ILogger<EntraClaimsTransformer> logger)
{
    public void Transform(TokenValidatedContext context)
    {
        // Transform - "scp": "gamestore_api.all",
        // To - "scope": "gamestore_api.all"
        var identity = context.Principal?.Identity as ClaimsIdentity;
        identity?.TransformScopeClaim(GameStoreClaimTypes.Scp);

        // "oid" claim value will be read and new claim will be created with userId -> "oid" claim value
        identity.MapUserIdClaim(GameStoreClaimTypes.Oid);

        // log new claims 
        context.Principal?.LogAllClaims(logger);
    }
}


