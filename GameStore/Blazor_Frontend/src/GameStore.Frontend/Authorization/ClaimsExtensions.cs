using System.Security.Claims;

namespace GameStore.Frontend.Authorization;

public static class ClaimsExtensions
{
    public static void TransformScopeClaim(this ClaimsIdentity? identity, string sourceScopeClaimType)
    {
        var scopeClaim = identity?.FindFirst(sourceScopeClaimType);

        if (scopeClaim is null)
        {
            return;
        }

        var scopes = scopeClaim.Value.Split(' ');

        identity?.RemoveClaim(scopeClaim);

        identity?.AddClaims(
            scopes.Select(
                scope => new Claim(GameStoreClaimTypes.Scope, scope)));
    }

    public static void MapUserIdClaim(this ClaimsIdentity? identity, string sourceClaimType)
    {
        var sourceClaim = identity?.FindFirst(sourceClaimType);

        if (sourceClaim != null)
        {
            identity?.AddClaim(new Claim(GameStoreClaimTypes.UserId, sourceClaim.Value));
        }
    }
}