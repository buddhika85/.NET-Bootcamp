using GameStore.Api.Data;
using GameStore.Api.Features.Games;
using GameStore.Api.Features.Genres;

var builder = WebApplication.CreateBuilder(args);

// EF CORE - SCOPED lifetime DB CONTEXT
var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

builder.Services.AddValidation(); // replaces WithParameterValidation() in all endpoints - now obsolete

// REGISTER SERVICES BEFORE BUILDING APPLICATION
// builder.Services.AddSingleton<GameStoreData>();
// builder.Services.AddTransient<GameStoreDataLogger>();



var app = builder.Build();




app.MapGames();
app.MapGenres();

// RUN MIGRATIONS WHEN APP STARTS & SEED
app.InitializeDb();

app.Run();