namespace GameStore.Api.Shared.FileUpload;

public class FileUploader(
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor)
{
    public const int MaxFileSize = 10 * 1024 * 1024;        // 10 MB
    public readonly string[] PermittedFileNames = [".jpg", ".jped", ".png"];


    public async Task<FileUploadResult> UploadFileAsync(
        IFormFile file,
        string folder)
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
        var uploadFolder = Path.Combine(environment.WebRootPath, folder);
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        // file name on server - no conflicts with other files
        var safeFileName = $"{Guid.NewGuid()}{fileExtension}";
        var fullPath = Path.Combine(uploadFolder, safeFileName);

        // stream copy
        using var stream = new FileStream(fullPath, FileMode.Create);           // destination to stream copy
        await file.CopyToAsync(stream);

        // uploaded file URL on server
        var httpContext = httpContextAccessor.HttpContext;
        uploadResult.FileUrl = $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}/{folder}/{safeFileName}";

        uploadResult.IsSuccess = true;
        return uploadResult;
    }
}
