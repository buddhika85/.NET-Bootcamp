using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GameStore.Api.Models;
using GameStore.Api.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace GameStore.Api.Features.Baskets.Authorization;

// Handler checks whether the AuthorizationRequirement is met or not
public class BasketAuthorizationHandler
    : AuthorizationHandler<OwnerOrAdminRequirement, CustomerBasket>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnerOrAdminRequirement requirement,
        CustomerBasket resource)
    {
        var currentUserId = context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(currentUserId))
        {
            return Task.CompletedTask;
        }

        if (Guid.Parse(currentUserId) == resource.Id    //  is it basket owner?
            || context.User.IsInRole(Roles.Admin))      // is it admin?
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}


// AuthorizationRequirement
public class OwnerOrAdminRequirement : IAuthorizationRequirement
{

}