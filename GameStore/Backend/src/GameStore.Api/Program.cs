using GameStore.Api.Data;
using GameStore.Api.Features.Games;
using GameStore.Api.Features.Genres;
using GameStore.Api.Shared.Timing;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);

// EF CORE - SCOPED lifetime DB CONTEXT
var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

builder.Services.AddValidation(); // replaces WithParameterValidation() in all endpoints - now obsolete

// REGISTER SERVICES BEFORE BUILDING APPLICATION
//builder.Services.AddTransient<RequestTimingMiddleware>();
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode |
                            HttpLoggingFields.Duration;
    options.CombineLogs = true;
});


var app = builder.Build();



// ROUTES
app.MapGames();
app.MapGenres();

// MIDDLEWARE
app.UseMiddleware<RequestTimingMiddleware>();
app.UseHttpLogging();



// RUN MIGRATIONS WHEN APP STARTS & SEED
await app.InitializeDbAsync();

app.Run();