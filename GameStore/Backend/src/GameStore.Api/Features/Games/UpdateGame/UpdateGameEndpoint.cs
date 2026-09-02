using GameStore.Api.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Features.Games.UpdateGame;

public static class UpdateGameEndpoint
{
    public static void MapUpdateGame(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}",
                async Task<Results<NotFound<string>, BadRequest<string>, NoContent>>
                ([FromRoute] Guid id, [FromBody] UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                var existingGame = await dbContext.Games.FindAsync(id);
                if (existingGame is null)
                    return TypedResults.NotFound($"Game with Id {id} not found");

                var genre = await dbContext.Genres.FindAsync(updatedGame.GenreId);
                if (genre is null)
                    return TypedResults.BadRequest($"Genre with Id {updatedGame.GenreId} not available");

                existingGame.Name = updatedGame.Name;
                existingGame.GenreId = updatedGame.GenreId;
                existingGame.Price = updatedGame.Price;
                existingGame.ReleaseDate = updatedGame.ReleaseDate;
                existingGame.Description = updatedGame.Description;

                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });
    }
}
