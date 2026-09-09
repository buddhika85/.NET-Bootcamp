using System.Security.Claims;
using GameStore.Api.Data;
using GameStore.Api.Models;
using GameStore.Api.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Features.Baskets.UpsertBasket;

public static class UpsertBasketEndpoint
{
    // PUT /baskets/user-id-guid
    public static void MapUpsertBastket(this IEndpointRouteBuilder app)
    {
        app.MapPut("/{userId:guid}", async

            ([FromRoute] Guid userId,
            [FromBody] UpsertBasketDto basket,
            GameStoreContext dbContext,
            IAuthorizationService authorizationService,
            ClaimsPrincipal user) =>
        {
            var usersBasket = await dbContext.Baskets
                                .Include(x => x.Items)
                                .FirstOrDefaultAsync(x => x.Id == userId);
            if (usersBasket is null)
            {
                // insert
                usersBasket = new CustomerBasket()
                {
                    Id = userId
                };
                await dbContext.Baskets.AddAsync(usersBasket);
            }
            else
            {
                // update
                // explicit delete basket items then re-insert new          
                dbContext.BasketItems.RemoveRange(usersBasket.Items);
            }

            // insert basket items
            await dbContext.BasketItems.AddRangeAsync(basket.Items.Select(x => new BasketItem
            {
                Id = Guid.CreateVersion7(),
                GameId = x.Id,
                CustomerBasketId = userId,
                Quantity = x.Quantity
            }));

            // only basket owner or Admin can upsert basket  
            var authResult = await authorizationService.AuthorizeAsync(
                user,
                userBasket,
                new OwnerOrAdminRequirement()
                );

            if (!authResult.Succeeded)
            {
                return TypedResults.Forbid();
            }

            await dbContext.SaveChangesAsync();
            return TypedResults.NoContent();
        })
        .RequireAuthorization(Policies.UserAccess);
    }
}
