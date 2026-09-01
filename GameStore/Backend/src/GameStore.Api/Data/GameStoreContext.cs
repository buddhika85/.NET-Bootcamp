using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

// dotnet ef migrations add InitialCreate --output-dir Data/Migrations
// dotnet ef database update

public class GameStoreContext
            (DbContextOptions<GameStoreContext> options)
            : DbContext(options)
{
    // DbSet is an optimized collection for relational table representations - code is built in to convert DbSet linq queries pure SQL
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Game> Games => Set<Game>();
}
