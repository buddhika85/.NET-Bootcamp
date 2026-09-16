using System.Security.Claims;

namespace GameStore.Api.Shared.Authorization;

public static class ClaimsExtensions
{
    public static void TransformScopeClaim(this ClaimsIdentity? identity, string sourceScopeClaimType)
    {
        var scopeClaim = identity?.FindFirst(sourceScopeClaimType);
        if (scopeClaim is null)
            return;

        // covert space seperate string of scopes to an array
        // "gamestore_api.all email profile" => [gamestore_api.all, email, profile]
        var scopes = scopeClaim.Value.Split(' ');

        // remove original scope claim
        identity?.RemoveClaim(scopeClaim);

        // re-add each scope claim 1 by 1 for each array element
        identity?.AddClaims(scopes.Select(x => new Claim(GameStoreClaimTypes.Scope, x)));
    }

    // Entra - "oid" claim value will be read and new claim will be created with userId -> "oid" claim value
    // KeyCloak - "Sub" claim value will be read and new claim will be created with userId -> "Sub" claim value
    public static void MapUserIdClaim(this ClaimsIdentity? identity, string sourceScopeClaimType)
    {
        var userIdClaim = identity?.FindFirst(sourceScopeClaimType);
        if (userIdClaim is not null)
        {
            identity?.AddClaim(new Claim(GameStoreClaimTypes.UserId, userIdClaim.Value));
        }
    }

    public static void LogAllClaims(this ClaimsPrincipal? principal, ILogger logger)
    {
        // log new claims 
        if (logger.IsEnabled(LogLevel.Trace))
        {
            var claims = principal?.Claims;
            if (claims is null)
                return;

            foreach (var claim in claims)
            {
                logger.LogTrace("- Claim: {ClaimType}, Value: {ClaimValue}",
                    claim.Type, claim.Value);
            }
        }
    }
}
