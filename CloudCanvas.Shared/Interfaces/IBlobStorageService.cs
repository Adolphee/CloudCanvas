using Azure.Storage.Blobs;
using CloudCanvas.Shared.DTOs;

namespace CloudCanvas.Shared.Interfaces;

/// <summary>
/// Defines methods for interacting with a blob storage service, including uploading files and retrieving file URLs from
/// a specified container.
/// </summary>
public interface IBlobStorageService
{
    public Task<BlobMetaDTO> UploadAsync(Stream fileStream, string filename, string containerName, string Id = "");
    public Task<List<string>> GetBlobUrlsAsync(string containerName);
    public Task<List<BlobMetaDTO>> GetBlobsAsync(string containerName);
    public Task<BlobContainerClient> GetOrCreateContainerClientAsync(string containerName, bool createIfNotExists = false);
    public Task<bool> DeleteAsync(string containerName, string blobName);
}
