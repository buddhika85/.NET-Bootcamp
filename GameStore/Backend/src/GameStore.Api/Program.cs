using GameStore.Api.Data;
using GameStore.Api.Features.Baskets;
using GameStore.Api.Features.Baskets.Authorization;
using GameStore.Api.Features.Games;
using GameStore.Api.Features.Genres;
using GameStore.Api.Shared.Authorization;
using GameStore.Api.Shared.ErrorHandling;
using GameStore.Api.Shared.FileUpload;
using GameStore.Api.Shared.Timing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

// PROBLEM DETAILS for ERRORS
builder.Services.AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();     // GLOBAL EXCEPTION HANDLER

// EF CORE - SCOPED lifetime DB CONTEXT
var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

builder.Services.AddValidation(); // replaces WithParameterValidation() in all endpoints - now obsolete

// REGISTER SERVICES BEFORE BUILDING APPLICATION
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;
    options.CombineLogs = true;
});

builder.Services.AddHttpContextAccessor()
                .AddSingleton<FileUploader>();

// OPEN API SERVICES
builder.Services.AddOpenApi();

// AUTHEN
builder.Services.AddAuthentication()           // Authentication - middlware and services added
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters.RoleClaimType = "role";
                });

// AUTHOR
builder.AddGameStoreAuthorization();    // Authorization - middlware and services added, policies defined in the extension method
builder.Services.AddSingleton<IAuthorizationHandler, BasketAuthorizationHandler>();

var app = builder.Build();



// ROUTES
app.MapGames();
app.MapGenres();
app.MapBaskets();

// OPEN API ROUTE - /openapi/v1.json
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();       // Register an endpoint onto the current application for resolving the OpenAPI document associated with the current application.
}

// MIDDLEWARE
app.UseMiddleware<RequestTimingMiddleware>();
app.UseHttpLogging();


// PRODUCTION RFC7007 - PROBLEM DETAIL Format
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
app.UseStatusCodePages();


app.UseStaticFiles();           // Serve Files from wwwroot folder

app.UseAuthorization();

// RUN MIGRATIONS WHEN APP STARTS & SEED
await app.InitializeDbAsync();




// TERMINAL MIDDLEWARE IN THE PIPELINE
app.Run();


// EXECUTE ENDPOINT SELECTED

// GO BACK TO APP.RUN TERMINAL MIDDLEWARE, & GO BOTTOM TO TOP OF MIDDLEWARE PIPELINE