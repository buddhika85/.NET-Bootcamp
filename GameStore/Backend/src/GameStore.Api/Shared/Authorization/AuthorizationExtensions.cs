using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GameStore.Api.Shared.Authorization;

public static class AuthorizationExtensions
{
    private const string ApiAccessScope = "gamestore_api.all";

    public static IHostApplicationBuilder AddGameStoreAuthentication(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<KeyCloakClaimsTransformer>();    // KeyCloak Claims Transformer
        builder.Services.AddAuthentication(Schemes.KeyCloak)           // Authentication - middlware and services added - use KeyCloak scheme by default
                        .AddJwtBearer(options =>
                        {
                            options.MapInboundClaims = false;
                            options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                        })
                        .AddJwtBearer(Schemes.KeyCloak, options =>
                        {
                            // options.Authority = "http://localhost:8080/realms/gamestore";
                            // options.Audience = "gamestore-api";

                            options.RequireHttpsMetadata = false;       // avoid HTTPs for Dev

                            options.MapInboundClaims = false;
                            options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;

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
        return builder;
    }

    public static IHostApplicationBuilder AddGameStoreAuthorization(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAuthorizationBuilder()     // Authorization - middlware and services added
                .AddFallbackPolicy(Policies.UserAccess, authBuilder =>
                {
                    authBuilder.RequireClaim(ClaimTypes.Scope, ApiAccessScope);
                })
                .AddPolicy(Policies.AdminAccess, authBuilder =>
                {
                    authBuilder.RequireClaim(ClaimTypes.Scope, ApiAccessScope);
                    authBuilder.RequireRole(Roles.Admin);
                });
        return builder;
    }
}
