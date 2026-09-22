using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static async Task InitializeDbAsync(this WebApplication app)
    {
        await app.MigrateDbAsync();
        await app.SeedDbAsync();

        app.Logger.LogInformation(18, "-------> DB Ready: Migrations completed and DB seeded");
    }

    // registering DB Context, using SQL Server connetion string in Dev and Managed Identity in Azure
    public static WebApplicationBuilder AddGameStoreMsSQL<TContext>(
        this WebApplicationBuilder builder,
        string connectionStringName
        ) where TContext : DbContext
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' is missing.");
        builder.Services.AddSqlServer<TContext>(connectionString);

        return builder;
    }

    // Runs migrations when application starts - without doing it using CLI - dotnet ef database update
    private static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        GameStoreContext dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        await dbContext.Database.MigrateAsync();
    }

    // Seed initial data
    private static async Task SeedDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        GameStoreContext dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();

        if (!dbContext.Genres.Any())
        {
            // add to DB Context in-memory object
            dbContext.Genres.AddRange([
                new() { Name = "Fighting"},
                new() { Name = "Kids and Family" },
                new() { Name = "Racing" },
                new() { Name = "Roleplaying" },
                new() { Name = "Sports" }
            ]);

            // commit changes to physical DB
            await dbContext.SaveChangesAsync();
        }
    }
}
