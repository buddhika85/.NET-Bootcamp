using Azure.Storage.Blobs;

namespace GameStore.Api.Shared.Cdn;

// Transforms az storage URLs to Az Front Door Urls --> For law latency in image loading  
public class CdnUrlTransformer(
    IConfiguration configuration,
    BlobServiceClient blobServiceClient)
{
    public string TransformToCdnUrl(string storageUrl)
    {
        ArgumentNullException.ThrowIfNull(storageUrl);

        var azFrontDoorHost = configuration["AZURE_FRONTDOOR_HOSTNAME"];        // Production-Only setting
        if (string.IsNullOrWhiteSpace(azFrontDoorHost))
        {
            // we are in local enviroment - return azurite url - no az blob here
            return storageUrl;
        }

        // transform storageUrl to dotnet Uri object for manpulation of host name
        if (Uri.TryCreate(storageUrl, UriKind.Absolute, out var uri))
        {
            // is it not a blob image URL ? - we only transform them, not all URLs
            var blobUrlHost = blobServiceClient.Uri.Host;
            if (!string.Equals(blobUrlHost, uri.Host, StringComparison.OrdinalIgnoreCase))
            {
                return storageUrl;
            }

            // if its a blob image url - replace blob image host with front door host
            var uriBuilder = new UriBuilder(uri)
            {
                Host = azFrontDoorHost
            };

            return uriBuilder.Uri.ToString();
        }

        return storageUrl;
    }
}
