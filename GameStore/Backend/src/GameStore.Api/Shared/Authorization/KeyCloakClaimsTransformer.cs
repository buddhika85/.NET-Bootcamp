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
        var scopeClaim = identity?.FindFirst(ClaimTypes.Scope);
        if (scopeClaim is null)
            return;

        // covert space seperate string of scopes to an array
        // "gamestore_api.all email profile" => [gamestore_api.all, email, profile]
        var scopes = scopeClaim.Value.Split(' ');

        // remove original scope claim
        identity?.RemoveClaim(scopeClaim);

        // re-add each scope claim 1 by 1 for each array element
        identity?.AddClaims(scopes.Select(x => new Claim(ClaimTypes.Scope, x)));


        // log new claims 
        if (logger.IsEnabled(LogLevel.Trace))
        {
            var claims = context.Principal?.Claims;
            if (claims is null)
                return;

            foreach (var claim in claims)
            {
                logger.LogTrace("After Applying KeyCloakClaimsTransformer - Claim: {ClaimType}, Value: {ClaimValue}",
                    claim.Type, claim.Value);
            }
        }
    }
}
