using GameStore.Api.Data;
using GameStore.Api.Features.Games.Constants;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore.Api.Features.Games.GetGameById;

public static class GetGameByIdEndPoint
{
    // GET /games/{id}
    public static void MapGetGameById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", Results<NotFound, Ok<GameDetailsDto>> (Guid id, GameStoreContext dbContext) =>
        {
            var game = dbContext.Games.Find(id);
            return game is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(
                    new GameDetailsDto(
                        game.Id,
                        game.Name,
                        game.GenreId,
                        game.Price,
                        game.ReleaseDate,
                        game.Description
                    )
                );
        }).WithName(EndpointNames.GetGameById);
    }
}