namespace GameStore.Api.Shared.Authorization;

public static class GameStoreClaimTypes
{
    public const string Role = "role";              // KeyCloak
    public const string Roles = "roles";            // Entra

    public const string Scope = "scope";             // KeyCloak
    public const string Scp = "scp";           // Entra

    public const string Oid = "oid";           // Entra - Object Id
    public const string UserId = "userId";          // KeyCloak & Entra
}
