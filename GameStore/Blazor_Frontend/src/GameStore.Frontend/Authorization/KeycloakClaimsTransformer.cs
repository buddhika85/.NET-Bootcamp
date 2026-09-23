using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace GameStore.Frontend.Authorization;

public class KeycloakClaimsTransformer
{
    public void Transform(TokenValidatedContext context)
        => Transform(context.Principal?.Identity as ClaimsIdentity);

    public void Transform(ClaimsIdentity? identity)
    {
        identity?.TransformScopeClaim(GameStoreClaimTypes.Scope);
        identity?.MapUserIdClaim(JwtRegisteredClaimNames.Sub);
    }
}
