using GameStore.Api.Data;
using GameStore.Api.Features.Games.Constants;
using GameStore.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Features.Games.CreateGame;

public static class CreateGameEndpoint
{
    // POST /games
    public static void MapCreateGame(this IEndpointRouteBuilder app)
    {
        app.MapPost("/",
                async ([FromBody] CreateGameDto game,
                GameStoreContext dbContext,
                ILogger<Program> logger) =>
            {
                var gameEntity = new Game
                {
                    Name = game.Name,
                    GenreId = game.GenreId,
                    Price = game.Price,
                    ReleaseDate = game.ReleaseDate,
                    Description = game.Description
                };

                await dbContext.Games.AddAsync(gameEntity);

                await dbContext.SaveChangesAsync();

                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("------->New Game: {GameName} with Price {GamePrice} Created", gameEntity.Name, game.Price);

                return TypedResults.CreatedAtRoute(
                    value: new GameDetailsDto(gameEntity.Id, game.Name, game.GenreId, game.Price, game.ReleaseDate),
                    routeName: EndpointNames.GetGameById,
                    routeValues: new { id = gameEntity.Id });
            });
    }
}