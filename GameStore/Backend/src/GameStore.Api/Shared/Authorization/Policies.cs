namespace GameStore.Api.Shared.Authorization;


// dotnet user-jwts create --scope "gamestore_api.all"
// dotnet user-jwts create --scope "gamestore_api.all" --role "Admin"
public static class Policies                                        // check Program.cs for claim requirements for each policyu
{
    public const string UserAccess = nameof(UserAccess);            // requires 'scope' claim with 'gamestore_api.all' value - Program.cs defines this
    public const string AdminAccess = nameof(AdminAccess);          // requires 'scope' claim with 'gamestore_api.all' value  + 'role' claim with 'Admin' value - Program.cs defines this
}
