using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Features.Genres.GetGenres;

public static class GetGenresEndpoint
{
    public static void MapGetGenres(this IEndpointRouteBuilder app)
    {
        app.MapGet("", async (GameStoreContext dbContext) =>
            TypedResults.Ok<IReadOnlyList<GenreDto>>(
                await dbContext
                    .Genres
                    .AsNoTracking()
                    .Select(x => new GenreDto(x.Id, x.Name))
                    .ToListAsync()
            ));
    }
}
