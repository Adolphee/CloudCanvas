using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Utilities;

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
            try
            {
                var bcClient = _client.GetBlobContainerClient(containerName);
                //if statement gives more control over creation, default true, returns zero if false && blob not exists
                if (createIfNotExists) await bcClient.CreateIfNotExistsAsync(); 
                return bcClient;
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
       /// <param name="fileStream">The stream containing the file data to upload. The stream must be readable and its position will be reset to
       /// 0 before uploading.</param>
       /// <param name="filename">The name of the file to be created in the blob storage. Cannot be null or empty.</param>
       /// <param name="containerName">The name of the blob storage container where the file will be uploaded. Defaults to the "Uploads" container
       /// if not specified. Cannot be null or empty.</param>
       /// <returns>A task that represents the asynchronous upload operation.</returns>
       /// <exception cref="BadImageFormatException">Thrown if <paramref name="fileStream"/> is null or cannot be read.</exception>
        public async Task UploadAsync(Stream fileStream, string filename, string containerName = BlobStorage.Containers.Uploads)
        {
            Validate.StringValue(nameof(filename), filename);
            Validate.StringValue(nameof(containerName), containerName);
            if (fileStream != null)
            {
                fileStream.Position = 0;
                try
                {
                    var client = await GetOrCreateContainerClientAsync(containerName);
                    var blob = client.GetBlobClient(filename);
                    await blob.UploadAsync(fileStream, true);
                }
                catch (Exception e) 
                {
                    _logger.LogError(e, "Failed to upload file {originalFileName} to {blobContainer}", filename, containerName);
                    throw; 
                }
            } else
            {
                throw new BadImageFormatException($"Unable to read file: {filename} from container {containerName}.");
            }
        }
    }
}