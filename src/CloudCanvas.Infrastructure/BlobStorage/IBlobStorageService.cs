using Azure.Storage.Blobs;
using CloudCanvas.Infrastructure.DTOs;

namespace CloudCanvas.Infrastructure.BlobStorage;

/// <summary>
/// Defines methods for interacting with a blob storage service, including uploading files and retrieving file URLs from
/// a specified container.
/// </summary>
public interface IBlobStorageService
{
    Task<BlobMetaDTO> UploadAsync(Stream fileStream, string filename, Dictionary<string, string> blobProperties, string containerName = BStorage.Containers.Uploads, string customIdentifier = null!);
    Task<List<string>> GetBlobUrlsAsync(string containerName);
    Task<List<BlobMetaDTO>> GetBlobsAsync(string containerName);
    Task<BlobMetaDTO> GetBlobMetadataAsync(string identifier, string fromContainer);
    Task<BlobMetaDTO> AddBlobMetadataAsync(BlobMetaDTO blob, string key, string value);
    Task<BlobContainerClient> GetOrCreateContainerClientAsync(string containerName, bool createIfNotExists = false);
    Task<bool> DeleteAsync(string containerName, string blobName);
}
