using Azure.Storage.Blobs;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using Microsoft.AspNetCore.Http;

namespace CloudCanvas.Application.Abstractions.Storage;

/// <summary>
/// Defines methods for interacting with a blob storage service, including uploading files and retrieving file URLs from
/// a specified container.
/// </summary>
public interface IBlobStorageService
{
    Task<UploadPhotoResult> UploadAsync(Stream fileStream, string filename, Dictionary<string, string> blobProperties, string containerName = BStorage.Containers.Uploads, string customIdentifier = null!);
    Task<UploadPhotoResult> UploadAsync(IFormFile file, Dictionary<string, string> props, string containerName = BStorage.Containers.Uploads, string customIdentifier = null!);
    Task<List<string>> GetBlobUrlsAsync(string containerName);
    Task<List<UploadPhotoResult>> GetBlobsAsync(string containerName);
    Task<UploadPhotoResult> GetBlobMetadataAsync(string identifier, string fromContainer);
    Task<UploadPhotoResult> AddBlobMetadataAsync(UploadPhotoResult blob, string key, string value);
    Task<BlobContainerClient> GetOrCreateContainerClientAsync(string containerName, bool createIfNotExists = false);
    Task<bool> DeleteAsync(string containerName, string blobName);
}
