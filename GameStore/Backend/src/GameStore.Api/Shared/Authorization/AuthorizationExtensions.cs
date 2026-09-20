using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;

namespace GameStore.Api.Shared.Authorization;

public static class AuthorizationExtensions
{
    private const string ApiAccessScope = "gamestore_api.all";

    public static IHostApplicationBuilder AddGameStoreAuthentication(this IHostApplicationBuilder builder)
    {

        var authBuilder = builder.Services.AddAuthentication(Schemes.KeyCloakOrEntra);           // Authentication - middlware and services added - use Entra scheme by default

        // in dev - we will have Entra & keyCloack both schemes
        // in prod - on Entra
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSingleton<KeyCloakClaimsTransformer>();    // KeyCloak Claims Transformer
            authBuilder.AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters.RoleClaimType = GameStoreClaimTypes.Role;
            })
            .AddJwtBearer(Schemes.KeyCloak, options =>
            {
                // options.Authority = "http://localhost:8080/realms/gamestore";
                // options.Audience = "gamestore-api";

                options.RequireHttpsMetadata = false;       // avoid HTTPs for Dev

                options.MapInboundClaims = false;
                options.TokenValidationParameters.RoleClaimType = GameStoreClaimTypes.Role;

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimTransformer = context.HttpContext
                                                .RequestServices
                                                .GetRequiredService<KeyCloakClaimsTransformer>();
                        claimTransformer.Transform(context);
                        return Task.CompletedTask;
                    }
                };
            });
        }

        builder.Services.AddSingleton<EntraClaimsTransformer>();    // Entra Claims Transformer
        authBuilder.AddJwtBearer(Schemes.Entra, options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters.RoleClaimType = GameStoreClaimTypes.Roles;
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var claimTransformer = context.HttpContext
                                            .RequestServices
                                            .GetRequiredService<EntraClaimsTransformer>();
                    claimTransformer.Transform(context);
                    return Task.CompletedTask;
                }
            };
        });

        // decides which Scheme to to use
        authBuilder.AddPolicyScheme(
                    Schemes.KeyCloakOrEntra,
                    Schemes.KeyCloakOrEntra,
                    options =>
                    {
                        options.ForwardDefaultSelector = context =>
                        {
                            // read authorisation header
                            string authorisationHeader = context.Request.Headers[HeaderNames.Authorization]!;
                            // check if its empty and starts with "Bearer " Header Prefix
                            if (!string.IsNullOrEmpty(authorisationHeader) && authorisationHeader.StartsWith("Bearer "))
                            {
                                // if so -> Remove " Header Prefix get auth token
                                var authToken = authorisationHeader["Bearer ".Length..].Trim();
                                // create JWTSecurityTokenHandler object
                                var jWTSecurityTokenHandler = new JwtSecurityTokenHandler();
                                // JWTSecurityTokenHandler can read roken and token isser contains "ciamlogin.com" - return Schemes.Entra else Schemes.keyCloak
                                return jWTSecurityTokenHandler.CanReadToken(authToken)
                                    && jWTSecurityTokenHandler.ReadJwtToken(authToken).Issuer.Contains("ciamlogin.com") ?
                                    Schemes.Entra :
                                    Schemes.KeyCloak;
                            }
                            // defaults to Entra
                            return Schemes.Entra;
                        };
                    });

        return builder;
    }

    public static IHostApplicationBuilder AddGameStoreAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()     // Authorization - middlware and services added
                .AddFallbackPolicy(Policies.UserAccess, authBuilder =>
                {
                    authBuilder.RequireClaim(GameStoreClaimTypes.Scope, ApiAccessScope);
                })
                .AddPolicy(Policies.AdminAccess, authBuilder =>
                {
                    authBuilder.RequireClaim(GameStoreClaimTypes.Scope, ApiAccessScope);
                    authBuilder.RequireRole(Roles.Admin);
                });
        return builder;
    }
}
