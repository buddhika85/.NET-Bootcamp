using Azure.Core;
using Azure.Storage.Blobs;

namespace GameStore.Api.Shared.FileUpload;

public static class FileUploadExtensions
{
    public static void AddFileUploader(this WebApplicationBuilder builder,
                                        TokenCredential credential)
    {
        builder.Services.AddSingleton(serviceProvider =>
                    {
                        // return new BlobServiceClient(builder.Configuration.GetConnectionString("Blobs"));

                        var config = serviceProvider.GetRequiredService<IConfiguration>();
                        var blobServiceConnString = config.GetConnectionString("Blobs")
                            ?? throw new InvalidOperationException("Storage URL is mising");

                        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();

                        return environment.IsDevelopment() ?
                            new BlobServiceClient(blobServiceConnString)
                            : new BlobServiceClient(
                                new Uri(blobServiceConnString),
                                credential);
                    })
                .AddSingleton<FileUploader>();
    }
}
