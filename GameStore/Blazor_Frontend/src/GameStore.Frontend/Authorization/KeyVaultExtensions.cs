using Azure.Identity;

namespace GameStore.Frontend.Authorization;

public static class KeyVaultExtensions
{
    public static void AddGameStoreKeyVault(this IHostApplicationBuilder builder)
    {
        var vaultUri = builder.Configuration.GetConnectionString("KeyVault") ??
            throw new InvalidOperationException("KeyVault is not set");

        builder.Configuration.AddAzureKeyVault(
                new Uri(vaultUri),
                new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = builder.Configuration["AZURE_CLIENT_ID"]
                }));
    }
}
