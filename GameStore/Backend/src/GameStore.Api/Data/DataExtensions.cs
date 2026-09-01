using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static void InitializeDb(this WebApplication app)
    {
        app.MigrateDb();
        app.SeedDb();
    }

    // Runs migrations when application starts - without doing it using CLI - dotnet ef database update
    private static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        GameStoreContext dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();
    }

    // Seed initial data
    private static void SeedDb(this WebApplication app)
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
            dbContext.SaveChanges();
        }
    }
}
