using GameStore.Api.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Features.Games.GetGames;

public static class GetGamesEndpoint
{
    // GET /games
    public static void MapGetGames(this IEndpointRouteBuilder app)
    {
        app.MapGet("/",
            async Task<Ok<GamesPageDto>> (
            [AsParameters] GetGamesDto request,
            GameStoreContext dbContext
            ) =>
        {
            // build with where clause if name provided
            var gamesQuery = dbContext.Games
                                        .Where(x =>
                                            string.IsNullOrWhiteSpace(request.Name) ||
                                                EF.Functions.Like(
                                                    x.Name.ToLower(),
                                                    $"%{request.Name.ToLower()}%"));

            // find total page count
            var totalRecords = await gamesQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (float)request.PageSize);

            // get paged records
            var skipCount = (request.PageNumber - 1) * request.PageSize;
            var paginatedQuery = gamesQuery
                                    .OrderBy(x => x.Name)
                                    .Include(x => x.Genre)                      // Eager Load Navigational Property                              
                                    .AsNoTracking()                             // No Tracking - Read-Only Query      
                                    .Skip(skipCount)
                                    .Take(request.PageSize)
                                    .Select(x => new GameSummaryDto(
                                        x.Id,
                                        x.Name,
                                        x.Genre!.Name,
                                        x.Price,
                                        x.ReleaseDate
                                    ));
            var gamesOnPage = await paginatedQuery.ToListAsync();

            return TypedResults.Ok(
                new GamesPageDto(
                    totalPages,
                    gamesOnPage
                    )
            );
        }


        );
    }
}