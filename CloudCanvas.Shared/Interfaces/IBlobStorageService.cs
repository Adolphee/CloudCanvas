namespace CloudCanvas.Interfaces;

/// <summary>
/// Defines methods for interacting with a blob storage service, including uploading files and retrieving file URLs from
/// a specified container.
/// </summary>
public interface IBlobStorageService
{
    public Task UploadAsync(string containerName, Stream fileStream, string filename);
    public Task<List<string>> GetUrlsAsync(string containerName);
}
