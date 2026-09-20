using Azure.Storage.Blobs;

namespace GameStore.Api.Shared.FileUpload;

public static class FileUploadExtensions
{
    public static void AddFileUploader(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(serviceProvider =>
                    {
                        // return new BlobServiceClient(builder.Configuration.GetConnectionString("Blobs"));

                        var config = serviceProvider.GetRequiredService<IConfiguration>();
                        var blobConnString = config.GetConnectionString("Blobs");
                        return new BlobServiceClient(blobConnString);
                    })
                .AddSingleton<FileUploader>();
    }
}
