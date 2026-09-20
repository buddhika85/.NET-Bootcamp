using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace GameStore.Api.Shared.FileUpload;

public class FileUploader(BlobServiceClient blobServiceClient)
{
    public const int MaxFileSize = 10 * 1024 * 1024;        // 10 MB
    public readonly string[] PermittedFileNames = [".jpg", ".jped", ".png"];

    public async Task<FileUploadResult> UploadFileAsync(
        IFormFile file,
        string folder)      // folder === azure blob container name
    {
        var uploadResult = new FileUploadResult();

        // validations - fail fast approach
        if (file == null || file.Length == 0)
        {
            uploadResult.IsSuccess = false;
            uploadResult.ErrorMessage = "No file uploaded";
            return uploadResult;
        }

        if (file.Length > MaxFileSize)
        {
            uploadResult.IsSuccess = false;
            uploadResult.ErrorMessage = "File is too large";
            return uploadResult;
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(fileExtension)
            || !PermittedFileNames.Contains(fileExtension))
        {
            uploadResult.IsSuccess = false;
            uploadResult.ErrorMessage = "Invalid file type";
            return uploadResult;
        }

        // create folder
        var containerClient = blobServiceClient.GetBlobContainerClient(folder);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        // file name on blob container - no conflicts with other files
        var safeFileName = $"{Guid.NewGuid()}{fileExtension}";
        var blobClient = containerClient.GetBlobClient(safeFileName);
        await blobClient.DeleteIfExistsAsync();

        // upload file as a stream to azure blob
        using var fileStream = file.OpenReadStream();
        await blobClient.UploadAsync(
            fileStream,
            new BlobHttpHeaders { ContentType = file.ContentType }
            );


        // file upload to azure blob is complete 
        uploadResult.IsSuccess = true;
        uploadResult.FileUrl = blobClient.Uri.ToString();
        return uploadResult;
    }
}
