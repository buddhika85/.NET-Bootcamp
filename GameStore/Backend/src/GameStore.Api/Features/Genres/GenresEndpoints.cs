using GameStore.Api.Features.Genres.GetGenres;

namespace GameStore.Api.Features.Genres;

public static class GenresEndpoints
{
    public static void MapGenres(this IEndpointRouteBuilder app)
    {
        // Apply the common prefix for all genres endpoint
        var group = app.MapGroup("/genres");

        // GET /genres/
        group.MapGetGenres();
    }
}
