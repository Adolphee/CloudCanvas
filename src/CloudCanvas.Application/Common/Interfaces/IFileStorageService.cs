using Azure.Storage.Blobs;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;

namespace CloudCanvas.Application.Common.Interfaces;

/// <summary>
/// Defines methods for interacting with a blob storage service, including uploading files and retrieving file URLs from
/// a specified container.
/// </summary>
public interface IFileStorageService
{
    Task<List<string>> GetFileUrlsAsync(string containerName, CancellationToken cancellation = default);
    Task<FileMetadata> UploadAsync(Stream fileStream, string filename, Dictionary<string, string> blobProperties, string containerName = BStorage.Containers.Uploads, string customIdentifier = null!, CancellationToken cancellation = default);
    Dictionary<string, string> SetOriginalMetadata(string filename, string uploadedById, CancellationToken cancellation = default);
    Task<FileMetadata> GetFileMetadataAsync(string identifier, string fromContainer, CancellationToken cancellation = default);
    Task<FileMetadata> AddFileMetadataAsync(FileMetadata file, string key, string value, CancellationToken cancellation = default);
    Task<List<FileMetadata>> GetFilesAsync(string containerName, CancellationToken cancellation = default);
    Task<bool> DeleteAsync(string containerName, string blobName, CancellationToken cancellation = default);
    Task<BlobContainerClient> GetOrCreateContainerClientAsync(string containerName, bool createIfNotExists = false, CancellationToken cancellation = default);
}
