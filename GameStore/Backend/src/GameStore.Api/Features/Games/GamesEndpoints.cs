using GameStore.Api.Features.Games.CreateGame;
using GameStore.Api.Features.Games.DeleteGame;
using GameStore.Api.Features.Games.GetGameById;
using GameStore.Api.Features.Games.GetGames;
using GameStore.Api.Features.Games.UpdateGame;

namespace GameStore.Api.Features.Games;

public static class GamesEndpoints
{
    public static void MapGames(this IEndpointRouteBuilder app)
    {
        // Apply the common prefix for all games endpoints
        var group = app.MapGroup("/games");

        // GET /games
        group.MapGetGames();

        // GET /games/{id}
        group.MapGetGameById();

        // POST /games
        group.MapCreateGame();

        // PUT /games/{id}
        group.MapUpdateGame();

        // DELETE /games/{id:guid}
        group.MapDeleteGame();
    }
}
