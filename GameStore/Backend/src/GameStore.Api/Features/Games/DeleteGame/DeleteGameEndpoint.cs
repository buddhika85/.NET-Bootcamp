using GameStore.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Features.Games.DeleteGame;

public static class DeleteGameEndpoint
{
    // DELETE /games/{id:guid}
    public static void MapDeleteGame(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/{id:guid}",
            async ([FromRoute] Guid id, GameStoreContext dbContext) =>
            {
                await dbContext.Games
                    .Where(x => x.Id == id)             // Filter which records to deletre
                    .ExecuteDeleteAsync();                   // Batch Delete directly in Physical DB

                return TypedResults.NoContent();
            });
    }
}
