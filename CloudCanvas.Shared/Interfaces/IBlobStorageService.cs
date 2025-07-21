using Azure.Storage.Blobs;

namespace CloudCanvas.Shared.Interfaces;

/// <summary>
/// Defines methods for interacting with a blob storage service, including uploading files and retrieving file URLs from
/// a specified container.
/// </summary>
public interface IBlobStorageService
{
    public Task UploadAsync(Stream fileStream, string filename, string containerName);
    public Task<List<string>> GetBlobUrlsAsync(string containerName);
    public Task<BlobContainerClient> GetOrCreateContainerClientAsync(string containerName, bool createIfNotExists = true);
}
