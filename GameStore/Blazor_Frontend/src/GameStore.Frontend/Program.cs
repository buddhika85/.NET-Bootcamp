using GameStore.Frontend.Clients;
using GameStore.Frontend.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();

var backendApiUrl = builder.Configuration["BackendApiUrl"] ??
    throw new Exception("BackendApiUrl is not set");

builder.Services.AddHttpClient<GamesClient>(
    client => client.BaseAddress = new Uri(backendApiUrl));

builder.Services.AddHttpClient<GenresClient>(
    client => client.BaseAddress = new Uri(backendApiUrl));

builder.Services.AddValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>();

app.Run();
