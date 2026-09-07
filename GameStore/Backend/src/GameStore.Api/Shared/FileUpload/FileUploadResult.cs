namespace GameStore.Api.Shared.FileUpload;

public class FileUploadResult
{
    public bool IsSuccess { get; set; }
    public string? FileUrl { get; set; }        // if upload fails, this url will be null
    public string? ErrorMessage { get; set; }   // if file upload is success, this error message will be null
}
