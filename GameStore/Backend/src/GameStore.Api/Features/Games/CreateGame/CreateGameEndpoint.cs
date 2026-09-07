using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GameStore.Api.Data;
using GameStore.Api.Features.Games.Constants;
using GameStore.Api.Models;
using GameStore.Api.Shared.FileUpload;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Features.Games.CreateGame;

public static class CreateGameEndpoint
{
    private const string DefaultImageUri = "https://placehold.co/100";
    // POST /games
    public static void MapCreateGame(this IEndpointRouteBuilder app)
    {
        app.MapPost("/",
                async Task<Results<
                    UnauthorizedHttpResult,
                    BadRequest<ErrorResponseDto>,
                    CreatedAtRoute<GameDetailsDto>>> (
                    [FromForm] CreateGameDto game,                  // cannot use [FromBody] - JSON, as this contains Image file, must use [FromForm]
                    GameStoreContext dbContext,
                    FileUploader fileUploader,
                    ILogger<Program> logger,
                    ClaimsPrincipal user) =>
            {
                if (user?.Identity?.IsAuthenticated == false)
                {
                    return TypedResults.Unauthorized();
                }

                var currentUserId = user?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return TypedResults.Unauthorized();
                }

                var imageUri = DefaultImageUri;
                if (game.ImageFile is not null)
                {
                    var fileUploadResult = await fileUploader.UploadFileAsync(
                                                                game.ImageFile,
                                                                StorageNames.GameImagesFolder);
                    if (!fileUploadResult.IsSuccess)
                    {
                        return TypedResults.BadRequest(new ErrorResponseDto(fileUploadResult.ErrorMessage!));
                    }
                    imageUri = fileUploadResult.FileUrl;
                }

                var gameEntity = new Game
                {
                    Name = game.Name,
                    GenreId = game.GenreId,
                    Price = game.Price,
                    ReleaseDate = game.ReleaseDate,
                    Description = game.Description,
                    ImageUri = imageUri!,
                    LastUpdatedBy = currentUserId
                };

                await dbContext.Games.AddAsync(gameEntity);

                await dbContext.SaveChangesAsync();

                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation("------->New Game: {GameName} with Price {GamePrice} Created",
                                            gameEntity.Name,
                                            game.Price);

                return TypedResults.CreatedAtRoute(
                    value: new GameDetailsDto(
                        gameEntity.Id,
                        gameEntity.Name,
                        gameEntity.GenreId,
                        gameEntity.Price,
                        gameEntity.ReleaseDate,
                        gameEntity.ImageUri,
                        gameEntity.LastUpdatedBy),
                    routeName: EndpointNames.GetGameById,
                    routeValues: new { id = gameEntity.Id });
            })
            .DisableAntiforgery();      // Since we use JWT, not cookies which are susseptable for CSRF attacks
    }
}