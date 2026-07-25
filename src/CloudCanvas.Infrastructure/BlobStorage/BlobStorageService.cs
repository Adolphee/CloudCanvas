using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CloudCanvas.Application.Abstractions.Storage;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail;
using CloudCanvas.Infrastructure.Common.Extensions;

namespace CloudCanvas.Infrastructure.BlobStorage
{
    /// <summary>
    /// Provides functionality for interacting with Azure Photo Storage, including retrieving blob container clients,
    /// enumerating blob URLs, and uploading files to blob storage.
    /// </summary>
    /// <remarks>This service acts as a wrapper around the <see cref="BlobServiceClient"/> to simplify common
    /// operations with Azure Photo Storage. It includes methods for retrieving blob container clients, listing blob
    /// URLs, and uploading files. The service ensures proper validation of input parameters and handles exceptions
    /// related to blob storage operations.</remarks>
    public class BlobStorageService: IMediaStorage
    { 
        private readonly BlobServiceClient _client;
        private BlobContainerClient _bcClient = default!;
        private readonly ILogger<BlobStorageService> _logger;
        public BlobStorageService(BlobServiceClient client, ILogger<BlobStorageService> logger)
        {
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously retrieves a <see cref="BlobContainerClient"/> for the specified container name. Optionally
        /// creates the container if it does not already exist.
        /// </summary>
        /// <remarks>This method validates the <paramref name="containerName"/> parameter and ensures it
        /// is not null or empty. If <paramref name="createIfNotExists"/> is <see langword="true"/>, the method attempts
        /// to create the container if it does not already exist. If the container creation fails or another error
        /// occurs, a  <see cref="BlobContainerClientInitializationFailedException"/> is thrown.</remarks>
        /// <param name="containerName">The name of the blob container to retrieve. Cannot be null or empty.</param>
        /// <param name="createIfNotExists">A boolean value indicating whether to create the container if it does not exist. If <see langword="true"/>,
        /// the container will be created if it does not already exist;  otherwise, the method will return the client
        /// for the container without creating it.</param>
        /// <returns>A <see cref="BlobContainerClient"/> instance for the specified container.</returns>
        /// <exception cref="BlobContainerClientInitializationFailedException">Thrown if an error occurs while initializing the <see cref="BlobContainerClient"/>.</exception>
        public async Task<BlobContainerClient> GetOrCreateContainerClientAsync(string containerName, bool createIfNotExists = true, CancellationToken cancellation = default)
        {
            if (_bcClient != default && _bcClient.Name == containerName) return _bcClient; // simple caching to avoid unnecessary calls to Azure
            try
            {
                _bcClient = _client.GetBlobContainerClient(containerName);
                //if statement gives more control over creation, default true, returns zero if false && blob not exists
                if (createIfNotExists) await _bcClient.CreateIfNotExistsAsync(); 
                return _bcClient;
            } catch (Exception e)
            {
                _logger.LogError(e, "Error: Failed to initiate new BlobContainerClient for container {0}", containerName);
                // This layer doesn’t know what to do with this low-level transport exception
                //  — let whoever owns the retry logic or orchestration deal with it.
                // SPOILER ALERT: that's me -_-'
                throw new BlobContainerClientInitializationFailedException(e.Message, e);
            }
        }

        /// <summary>
        /// Retrieves a list of URLs for all blobs in the specified container.
        /// </summary>
        /// <remarks>This method asynchronously enumerates all blobs in the specified container and
        /// constructs their URLs. Ensure that the container name provided is valid and accessible.</remarks>
        /// <param name="containerName">The name of the container from which to retrieve blob URLs.  This parameter cannot be null or empty.</param>
        /// <returns>A task that represents the asynchronous operation.  The task result contains a list of strings, where each
        /// string is the URL of a blob in the specified container. If the container is empty, the returned list will be
        /// empty.</returns>
        public async Task<List<string>> GetFileUrlsAsync(string containerName, CancellationToken cancellation = default)
        {
            var urls = new List<string>();
            var containerClient = await GetOrCreateContainerClientAsync(containerName);
            await foreach (var item in containerClient.GetBlobsAsync())
            {
                var blob = containerClient.GetBlobClient(item.Name);
                urls.Add(blob.Uri.ToString());
            }
            return urls;
        }

       /// <summary>
       /// Uploads a file to the specified blob storage container.
       /// </summary>
       /// <remarks>This method uploads the provided file memStream to the specified container in blob
       /// storage.  If a blob with the same name already exists, it will be overwritten.</remarks>
       /// <param name="filestream">The memStream containing the file data to upload. The memStream must be readable and its position will be reset to
       /// 0 before uploading.</param>
       /// <param name="filename">The name of the file to be created in the blob storage. Cannot be null or empty.</param>
       /// <param name="containerName">The name of the blob storage container where the file will be uploaded. Defaults to the "Uploads" container
       /// if not specified. Cannot be null or empty.</param>
       /// <returns>A task that represents the asynchronous upload operation.</returns>
       /// <exception cref="BadImageFormatException">Thrown if <paramref name="filestream"/> is null or cannot be read.</exception>
        public async Task<FileMetadata> UploadAsync(Stream filestream, string filename, Dictionary<string, string> blobProperties, string containerName = BStorage.Containers.Uploads, string customIdentifier = null!, CancellationToken cancellation = default)
        {
            filestream.Position = 0;
            var identifier = !String.IsNullOrWhiteSpace(customIdentifier) ? customIdentifier : Guid.NewGuid().ToString();
            return await UploadToBlobStorage(containerName, identifier, filestream, blobProperties, cancellation);
        }

        private async Task<FileMetadata> UploadToBlobStorage(string containerName, string identifier, Stream fileStream, Dictionary<string, string> metadata, CancellationToken cancellation = default)
        {
            try
            {
                var client = await GetOrCreateContainerClientAsync(containerName, default, cancellation);
                var blob = client.GetBlobClient($"{identifier}.jpeg");
                var info = await blob.UploadAsync(fileStream, new BlobUploadOptions { Metadata = metadata }, cancellation);
                var props = (await blob.GetPropertiesAsync(default, cancellation)).Value;
                FileMetadata dto = props.ToMetadata(identifier, blob.Uri.ToString());
                return dto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Upload failed {originalFileName} to {blobContainer}", metadata[BStorage.Meta.OriginalFilename], containerName);
                throw;
            }
        }

        /// <summary>
        /// Constructs a dictionary of metadata Properties for a blob, including the original filename, the ID of the user who uploaded the file, and the creation timestamp.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="uploadedById"></param>
        /// <param name="createdOn"></param>
        /// <returns type="Dictionary<string, string>">Dictionary of metadata Properties</returns>
        public Dictionary<string, string> SetOriginalMetadata(string filename, string uploadedById)
        {
            Dictionary<string, string> properties = new();
            properties.Add(BStorage.Meta.OriginalFilename, filename); // this is to enforce data consistency, convertability between BlobProperties & FileMetadata
            properties.Add(BStorage.Meta.UploadedBy, uploadedById); // Idem dito, these blob metadata are not available OOTB (afaik)
            properties.Add(BStorage.Meta.CreatedOn, DateTime.UtcNow.ToString());
            return properties;
        }

        public async Task<FileMetadata> GetFileMetadataAsync(string identifier, string fromContainer, CancellationToken cancellation = default)
        {
            var container = await GetOrCreateContainerClientAsync(fromContainer);
            var bclient = container.GetBlobClient(identifier);
            var props = (await bclient.GetPropertiesAsync(default, cancellation)).Value;
            return props.ToMetadata(identifier, bclient.Uri.ToString());
        }

        public async Task<FileMetadata> AddFileMetadataAsync(FileMetadata blob, string key, string value, CancellationToken cancellation = default)
        {
            blob.Metadata[key] = value;
            var bclient = await GetOrCreateContainerClientAsync(blob.ContainerName);
            await bclient.GetBlobClient(blob.Name).SetMetadataAsync(blob.Metadata);
            return blob;
        }

        public async Task<List<FileMetadata>> GetFilesAsync(string containerName, CancellationToken cancellation = default)
        {
            var container = _client.GetBlobContainerClient(containerName);
            var blobItems = container.GetBlobsAsync();
            List<FileMetadata> results = new();
            await foreach(var item in blobItems)
            {
               var blob = container.GetBlobClient(item.Name);
                FileMetadata meta = (await blob.GetPropertiesAsync()).Value.ToMetadata(blob.Name, blob.Uri.ToString());
                results.Add(meta);
            }
            return results;
        }
        

        public async Task<bool> DeleteAsync(string containerName, string identifier, CancellationToken cancellation = default)
        {
            var bclient = _client.GetBlobContainerClient(containerName).GetBlobClient(identifier);
            try
            {
                return await bclient.DeleteIfExistsAsync();
            } 
            catch (Exception e) when (e is RequestFailedException|| e is AggregateException)
            {
                _logger.LogError(e, "Failed to delete blob with name/identifier '{name}' from container '{container}'", identifier, containerName);
                return false;
            }
        }

        public async Task<Stream> GetFileStreamFromCommandAsync(CreateThumbnailCommand command, CancellationToken cancellation = default)
        {
            var bclient = await GetOrCreateContainerClientAsync(command.OriginalContainer ?? throw new InvalidArgumentException("Command.OriginalContainer cannot be null.")); // original file blob container
            var fileName = command.Photo.Location.Split("/").Last();
            var containerName = command.Photo.Location.Split("/").SkipLast(1).Last();
            await using var blobStream = await bclient.GetBlobClient(fileName).OpenReadAsync(); // download file
            var memStream = new MemoryStream(); // No using clause because I expect to return it
            await blobStream.CopyToAsync(memStream, cancellation);
            memStream.Position = 0;
            return memStream;
        }
    }
}