using Azure.Identity;
using GameStore.Api.Data;
using GameStore.Api.Features.Baskets;
using GameStore.Api.Features.Baskets.Authorization;
using GameStore.Api.Features.Games;
using GameStore.Api.Features.Genres;
using GameStore.Api.Shared.Authorization;
using GameStore.Api.Shared.Cdn;
using GameStore.Api.Shared.ErrorHandling;
using GameStore.Api.Shared.FileUpload;
using GameStore.Api.Shared.Timing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// DEFAULT CREDENTIAL
var defaultCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
{
    ManagedIdentityClientId = builder.Configuration["AZURE_CLIENT_ID"]             // User assigned managed identity
});

// PROBLEM DETAILS for ERRORS
builder.Services.AddProblemDetails()
                .AddExceptionHandler<GlobalExceptionHandler>();     // GLOBAL EXCEPTION HANDLER

// EF CORE - SCOPED lifetime DB CONTEXT
builder.AddGameStoreMsSQL<GameStoreContext>("GameStoreDB");

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

// FILE UPLOADER TO AZURE BLOB
builder.AddFileUploader(defaultCredential);

// OPEN API SERVICES
builder.Services.AddOpenApi();

// AUTHEN
builder.AddGameStoreAuthentication();

// AUTHOR
builder.AddGameStoreAuthorization();    // Authorization - middlware and services added, policies defined in the extension method
builder.Services.AddSingleton<IAuthorizationHandler, BasketAuthorizationHandler>();

// AZURE FRONT DOOR CDN URLs 
builder.Services.AddSingleton<CdnUrlTransformer>();

// AZURE SERVICES LOGGING
builder.Services.AddSingleton<AzureEventSourceLogForwarder>();

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
    // AZURE SERVICES LOGGING
    app.Services.GetRequiredService<AzureEventSourceLogForwarder>().Start();


    app.UseExceptionHandler();
}
app.UseStatusCodePages();


// app.UseStaticFiles();           // Serve Files from wwwroot folder - no need - now we use Azure Blobs not files inside wwwroot

app.UseAuthorization();

// RUN MIGRATIONS WHEN APP STARTS & SEED
await app.InitializeDbAsync();




// TERMINAL MIDDLEWARE IN THE PIPELINE
app.Run();


// EXECUTE ENDPOINT SELECTED

// GO BACK TO APP.RUN TERMINAL MIDDLEWARE, & GO BOTTOM TO TOP OF MIDDLEWARE PIPELINE