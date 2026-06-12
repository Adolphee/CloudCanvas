using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static CloudCanvas.Shared.Constants.BlobStorage;

namespace CloudCanvas.Shared.Services
{
    /// <summary>
    /// Provides functionality for interacting with Azure Blob Storage, including retrieving blob container clients,
    /// enumerating blob URLs, and uploading files to blob storage.
    /// </summary>
    /// <remarks>This service acts as a wrapper around the <see cref="BlobServiceClient"/> to simplify common
    /// operations with Azure Blob Storage. It includes methods for retrieving blob container clients, listing blob
    /// URLs, and uploading files. The service ensures proper validation of input parameters and handles exceptions
    /// related to blob storage operations.</remarks>
    public class BlobStorageService: IBlobStorageService
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
        public async Task<BlobContainerClient> GetOrCreateContainerClientAsync(string containerName, bool createIfNotExists = true)
        {
            Validate.StringValue(nameof(containerName),containerName);
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
        public async Task<List<string>> GetBlobUrlsAsync(string containerName)
        {
            var items = new List<string>();
            var blobClient = await GetOrCreateContainerClientAsync(containerName);
            await foreach (var item in blobClient.GetBlobsAsync())
            {
                var blob = blobClient.GetBlobClient(item.Name);
                items.Add(blob.Uri.ToString());
            }
            return items;
        }

       /// <summary>
       /// Uploads a file to the specified blob storage container.
       /// </summary>
       /// <remarks>This method uploads the provided file stream to the specified container in blob
       /// storage.  If a blob with the same name already exists, it will be overwritten.</remarks>
       /// <param name="filestream">The stream containing the file data to upload. The stream must be readable and its position will be reset to
       /// 0 before uploading.</param>
       /// <param name="filename">The name of the file to be created in the blob storage. Cannot be null or empty.</param>
       /// <param name="containerName">The name of the blob storage container where the file will be uploaded. Defaults to the "Uploads" container
       /// if not specified. Cannot be null or empty.</param>
       /// <returns>A task that represents the asynchronous upload operation.</returns>
       /// <exception cref="BadImageFormatException">Thrown if <paramref name="filestream"/> is null or cannot be read.</exception>
        public async Task<BlobMetaDTO> UploadAsync(Stream filestream, string filename, Dictionary<string, string> blobProperties, string containerName = BlobStorage.Containers.Uploads, string customIdentifier = null!)
        {
            filestream.Position = 0;
            var identifier = !String.IsNullOrWhiteSpace(customIdentifier) ? customIdentifier : Guid.NewGuid().ToString();
            blobProperties.Add("container", containerName);
            return await UploadToBlobStorage(containerName, identifier, filestream, blobProperties);
        }

        private async Task<BlobMetaDTO> UploadToBlobStorage(string containerName, string identifier, Stream fileStream, Dictionary<string, string> metadata)
        {
            try
            {
                var client = await GetOrCreateContainerClientAsync(containerName);
                var blob = client.GetBlobClient(identifier);
                var info = await blob.UploadAsync(fileStream, new BlobUploadOptions { Metadata = metadata });
                BlobMetaDTO dto = CCSerializer.MetaFromBlobProperties(identifier, blob.Uri.ToString(), blob.GetProperties());
                return dto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Upload failed {originalFileName} to {blobContainer}", metadata[BlobStorage.Meta.OriginalFilename], containerName);
                throw;
            }
        }

        public async Task<BlobMetaDTO> UploadAsync(IFormFile file, Dictionary<string, string> props, string containerName = BlobStorage.Containers.Uploads, string identifier = null!)
        {
            if (file != null)
            {
                var fileStream = file.OpenReadStream();
                fileStream.Position = 0;
                identifier = !String.IsNullOrWhiteSpace(identifier) ? identifier : Guid.NewGuid().ToString();
                props.Add("container", containerName);
                return await UploadToBlobStorage(containerName, identifier, fileStream, props ?? new());
            } else
            {
                throw new BadImageFormatException($"Upload failed: No file provided. (NULL FILE)");
            }
        }

        /// <summary>
        /// Constructs a dictionary of metadata properties for a blob, including the original filename, the ID of the user who uploaded the file, and the creation timestamp.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="uploadedById"></param>
        /// <param name="createdOn"></param>
        /// <returns type="Dictionary<string, string>">Dictionary of metadata properties</returns>
        public static Dictionary<string, string> SetOriginalMetadata(string filename, string uploadedById)
        {
            Dictionary<string, string> properties = new();
            properties.Add(BlobStorage.Meta.OriginalFilename, filename); // this is to enforce data consistency, convertability between BlobProperties & BlobMetaDTO
            properties.Add(BlobStorage.Meta.UploadedBy, uploadedById); // Idem dito, these blob metadata are not available OOTB (afaik)
            properties.Add(BlobStorage.Meta.CreatedOn, DateTime.UtcNow.ToString());
            return properties;
        }

        public async Task<BlobMetaDTO> GetBlobMetaAsync(string identifier, string fromContainer)
        {
            var container = await GetOrCreateContainerClientAsync(fromContainer);
            var bclient = container.GetBlobClient(identifier);
            var props = await bclient.GetPropertiesAsync();
            return CCSerializer.MetaFromBlobProperties(identifier, bclient.Uri.ToString(), props);
        }

        public async Task<BlobMetaDTO> AddMetadataAsync(BlobMetaDTO blob, string key, string value)
        {
            blob.Metadata[key] = value;
            var bclient = await GetOrCreateContainerClientAsync(blob.ContainerName);
            await bclient.GetBlobClient(blob.Name).SetMetadataAsync(blob.Metadata);
            return blob;
        }

        public async Task<List<BlobMetaDTO>> GetBlobsAsync(string containerName)
        {
            var container = _client.GetBlobContainerClient(containerName);
            var blobItems = container.GetBlobsAsync();
            List<BlobMetaDTO> results = new();
            await foreach(var item in blobItems)
            {
               var blob = container.GetBlobClient(item.Name);
                BlobMetaDTO meta = CCSerializer.MetaFromBlobProperties(blob.Name, blob.Uri.ToString(), await blob.GetPropertiesAsync());
                results.Add(meta);
            }
            return results;
        }
        

        public async Task<bool> DeleteAsync(string containerName, string identifier)
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
    }
}