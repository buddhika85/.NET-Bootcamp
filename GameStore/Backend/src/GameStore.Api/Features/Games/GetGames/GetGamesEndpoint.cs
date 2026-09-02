using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Features.Games.GetGames;

public static class GetGamesEndpoint
{
    // GET /games
    public static void MapGetGames(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", async (GameStoreContext dbContext) =>
            TypedResults.Ok<IReadOnlyList<GameSummaryDto>>(
                await dbContext
                        .Games
                        .Include(x => x.Genre)                  // Eager Load Navigational Property
                        .AsNoTracking()                         // No Tracking - Read-Only Query
                        .Select(x => new GameSummaryDto(
                                            x.Id,
                                            x.Name,
                                            x.Genre!.Name,
                                            x.Price,
                                            x.ReleaseDate))
                        .ToListAsync()

            )
        );
    }
}