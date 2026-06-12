using Azure.Storage.Blobs;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using Microsoft.AspNetCore.Http;

namespace CloudCanvas.Shared.Interfaces;

/// <summary>
/// Defines methods for interacting with a blob storage service, including uploading files and retrieving file URLs from
/// a specified container.
/// </summary>
public interface IBlobStorageService
{
    Task<BlobMetaDTO> UploadAsync(Stream fileStream, string filename, Dictionary<string, string> blobProperties, string containerName = BlobStorage.Containers.Uploads, string customIdentifier = null!);
    Task<BlobMetaDTO> UploadAsync(IFormFile file, Dictionary<string, string> props, string containerName = BlobStorage.Containers.Uploads, string customIdentifier = null!);
    Task<List<string>> GetBlobUrlsAsync(string containerName);
    Task<List<BlobMetaDTO>> GetBlobsAsync(string containerName);
    Task<BlobMetaDTO> GetBlobMetaAsync(string identifier, string fromContainer);
    Task<BlobMetaDTO> AddMetadataAsync(BlobMetaDTO blob, string key, string value);
    Task<BlobContainerClient> GetOrCreateContainerClientAsync(string containerName, bool createIfNotExists = false);
    Task<bool> DeleteAsync(string containerName, string blobName);
}
