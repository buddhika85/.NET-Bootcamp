using System.Security.Claims;
using GameStore.Api.Data;
using GameStore.Api.Features.Baskets.Authorization;
using GameStore.Api.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Features.Baskets.GetBasket;

public static class GetBasketEndpoint
{
    // GET /baskets/user-id-guid
    public static void MapGetBasket(this IEndpointRouteBuilder app)
    {
        app.Map("/{userId:guid}",
            async
            Task<Results<BadRequest, ForbidHttpResult, Ok<BasketDto>>>
            ([FromRoute] Guid userId,
            GameStoreContext dbContext,
            IAuthorizationService authorizationService,
            ClaimsPrincipal user) =>
        {
            if (userId == Guid.Empty)
            {
                return TypedResults.BadRequest();
            }

            var userBasket = await dbContext.Baskets
                .Include(x => x.Items)
                    .ThenInclude(i => i.Game)
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? new() { Id = userId };

            // only basket owner or Admin can read basket  
            var authResult = await authorizationService.AuthorizeAsync(
                user,
                userBasket,
                new OwnerOrAdminRequirement()
                );

            if (!authResult.Succeeded)
            {
                return TypedResults.Forbid();
            }

            var basket = new BasketDto(userId,
                userBasket.Items
                .Select(x =>
                    new BasketItemDto(
                        x.GameId,
                        x.Game!.Name,
                        x.Game!.Price,
                        x.Quantity,
                        x.Game.ImageUri))
                    .OrderBy(x => x.Name));
            return TypedResults.Ok(basket);
        })
        .RequireAuthorization(Policies.UserAccess);            // check program.cs for claims for this UserAccess policy
    }
}
