using Azure.Storage.Blobs;
using CloudCanvas.Constants;
using CloudCanvas.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.ColorSpaces;
using System.Runtime.CompilerServices;

namespace CloudCanvas.Services
{
    public class BlobStorageService: IBlobStorageService
    {
        private readonly IConfiguration _config;

        private readonly ILogger<BlobStorageService> _logger;
        public BlobStorageService(IConfiguration config, ILogger<BlobStorageService> logger)
        {
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Asynchronously retrieves a <see cref="BlobContainerClient"/> for the specified container name. If the
        /// container does not exist, it will be created.
        /// </summary>
        /// <remarks>This method uses the connection string configured for Azure Blob Storage to
        /// initialize the client. Ensure that the connection string is properly configured in the application's
        /// settings.</remarks>
        /// <param name="containerName">The name of the blob container to retrieve or create. Must not be null or empty.</param>
        /// <returns>A <see cref="BlobContainerClient"/> instance representing the specified container.</returns>
        /// <exception cref="InvalidDataException">Thrown if <paramref name="containerName"/> is null or empty.</exception>
        /// <exception cref="OperationCanceledException">Thrown if an error occurs while attempting to create or retrieve the container.</exception>
        public async Task<BlobContainerClient> GetContainerClientAsync(string containerName)
        {
            if (String.IsNullOrEmpty(containerName)) 
                throw new InvalidDataException("ContainerName not provided.");
            try
            {
                var cstring = _config.GetConnectionString(AzureBlobStorage.Self);
                var blobClient = new BlobContainerClient(cstring, containerName);
                await blobClient.CreateIfNotExistsAsync();
                return blobClient;
            } catch (Exception e)
            {
                _logger.LogError($"({e.Message}).\nException stacktrace: {e.StackTrace}");
                throw new OperationCanceledException("Error: Failed to initiate new BlobContainerClient.");
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
        public async Task<List<string>> GetUrlsAsync(string containerName)
        {
            var items = new List<string>();
            var blobClient = await GetContainerClientAsync(containerName);
            await foreach (var item in blobClient.GetBlobsAsync())
            {
                var blob = blobClient.GetBlobClient(item.Name);
                items.Add(blob.Uri.ToString());
            }
            return items;
        }

        /// <summary>
        /// Uploads a file to an Azure Blob Storage container.
        /// </summary>
        /// <remarks>This method uploads the provided file to an Azure Blob Storage container. If the
        /// container does not exist,  it will be created automatically. The file is uploaded with the specified
        /// filename, overwriting any existing  blob with the same name.</remarks>
        /// <param name="fileStream">The stream containing the file data to upload. Cannot be null.</param>
        /// <param name="filename">The name of the file to be stored in the blob container. Must be a valid filename.</param>
        /// <param name="fileType">The type of the file being uploaded, typically used for validation or metadata purposes.</param>
        /// <returns>A task that represents the asynchronous upload operation.</returns>
        /// <exception cref="BadImageFormatException">Thrown if <paramref name="fileStream"/> is null or does not contain valid image data.</exception>
        public async Task UploadAsync(string containerName, Stream fileStream, string filename)
        {
            if (fileStream != null)
            {
                fileStream.Position = 0;
                try
                {
                    var client = await GetContainerClientAsync(containerName ?? AzureBlobStorage.Containers.Uploads);
                    var blob = client.GetBlobClient(filename);
                    await blob.UploadAsync(fileStream, true);
                }
                catch (Exception e)
                {
                    _logger.LogError($"({e.Message}).\nException stacktrace: {e.StackTrace}");
                }
            } else
            {
                _logger.LogError($"[{typeof(BlobStorageService)}][{containerName}][{filename}]\nAttempted to upload '{filename}' but Stream was unavailable.");
                throw new BadImageFormatException($"Unable to read image: '{filename}'.");
            }
        }
    }
}