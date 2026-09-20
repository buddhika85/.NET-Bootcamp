using GameStore.Api.Data;
using GameStore.Api.Features.Games.Constants;
using GameStore.Api.Shared.Cdn;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore.Api.Features.Games.GetGameById;

public static class GetGameByIdEndPoint
{
    // GET /games/{id}
    public static void MapGetGameById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", async
            Task<Results<NotFound, Ok<GameDetailsDto>, ProblemHttpResult>> (
                Guid id,
                GameStoreContext dbContext,
                CdnUrlTransformer cdnUrlTransformer) =>
        {

            var game = await dbContext.Games.FindAsync(id); ;
            return game is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(
                            new GameDetailsDto(
                                game.Id,
                                game.Name,
                                game.GenreId,
                                game.Price,
                                game.ReleaseDate,
                                game.Description,
                                cdnUrlTransformer
                                    .TransformToCdnUrl(game.ImageUri),
                                game.LastUpdatedBy
                            )
                        );

        })
        .WithName(EndpointNames.GetGameById)
        .AllowAnonymous();
    }

}