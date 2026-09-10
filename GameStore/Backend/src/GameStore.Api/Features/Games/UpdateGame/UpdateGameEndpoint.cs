using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GameStore.Api.Data;
using GameStore.Api.Features.Games.Constants;
using GameStore.Api.Shared.Authorization;
using GameStore.Api.Shared.FileUpload;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Features.Games.UpdateGame;

public static class UpdateGameEndpoint
{
    public static void MapUpdateGame(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:guid}",
                async Task<Results<
                UnauthorizedHttpResult,
                NotFound<string>,
                BadRequest<ErrorResponseDto>,
                NoContent>>
                ([FromRoute] Guid id,
                [FromForm] UpdateGameDto updatedGame,
                GameStoreContext dbContext,
                FileUploader fileUploader,
                ClaimsPrincipal user) =>
            {
                var currentUserId = user?.FindFirstValue(JwtRegisteredClaimNames.Email)             // email or
                                        ?? user?.FindFirstValue(JwtRegisteredClaimNames.Sub);       // user Id guid

                if (string.IsNullOrEmpty(currentUserId))
                {
                    return TypedResults.Unauthorized();
                }

                var existingGame = await dbContext.Games.FindAsync(id);
                if (existingGame is null)
                    return TypedResults.NotFound($"Game with Id {id} not found");

                var genre = await dbContext.Genres.FindAsync(updatedGame.GenreId);
                if (genre is null)
                    return TypedResults.BadRequest(new ErrorResponseDto(
                        $"Genre with Id {updatedGame.GenreId} not available"));

                if (updatedGame.ImageFile is not null)
                {
                    var fileUploadResult = await fileUploader.UploadFileAsync(
                                                                updatedGame.ImageFile,
                                                                StorageNames.GameImagesFolder);
                    if (!fileUploadResult.IsSuccess)
                    {
                        return TypedResults.BadRequest(new ErrorResponseDto(fileUploadResult.ErrorMessage!));
                    }
                    existingGame.ImageUri = fileUploadResult.FileUrl!;
                }

                existingGame.Name = updatedGame.Name;
                existingGame.GenreId = updatedGame.GenreId;
                existingGame.Price = updatedGame.Price;
                existingGame.ReleaseDate = updatedGame.ReleaseDate;
                existingGame.Description = updatedGame.Description;
                existingGame.LastUpdatedBy = currentUserId;

                await dbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.AdminAccess);
    }
}
