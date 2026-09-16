using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace GameStore.Api.Shared.Authorization;

public static class AuthorizationExtensions
{
    private const string ApiAccessScope = "gamestore_api.all";

    public static IHostApplicationBuilder AddGameStoreAuthentication(this IHostApplicationBuilder builder)
    {

        var authBuilder = builder.Services.AddAuthentication(Schemes.Entra);           // Authentication - middlware and services added - use Entra scheme by default

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
