using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GameStore.Api.Shared.Authorization;

public class KeyCloakClaimsTransformer(ILogger<KeyCloakClaimsTransformer> logger)
{
    public void Transform(TokenValidatedContext context)
    {
        // Transform - "scope": "gamestore_api.all email profile", 
        // To - "scope": ["gamestore_api.all", "email", "profile"]
        var identity = context.Principal?.Identity as ClaimsIdentity;
        identity?.TransformScopeClaim(GameStoreClaimTypes.Scope);

        // "Sub" claim value will be read and new claim will be created with userId -> "Sub" claim value
        identity?.MapUserIdClaim(JwtRegisteredClaimNames.Sub);

        // log new claims 
        context.Principal?.LogAllClaims(logger);
    }
}


