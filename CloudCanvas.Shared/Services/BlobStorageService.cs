using Azure.Storage.Blobs;
using CloudCanvas.Constants;
using CloudCanvas.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.ColorSpaces;

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

        public async Task<BlobContainerClient> GetContainerClientAsync(string containerName)
        {
            if (String.IsNullOrEmpty(containerName)) throw new InvalidDataException("ContainerName not provided.");
            
            try
            {
                var cfg = _config.GetSection(AzureBlobStorage.Self);
                var cstring = cfg[AzureBlobStorage.ConnectionString];
                var blobClient = new BlobContainerClient(cstring, containerName);
                await blobClient.CreateIfNotExistsAsync();
                return blobClient;
            } catch (Exception e)
            {
                _logger.LogError($"({e.Message}).\nException stacktrace: {e.StackTrace}");
                throw new OperationCanceledException("Error: Failed to initiate new BlobContainerClient.");
            }
        }

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
            if (fileStream == null)
            {
                _logger.LogError($"Attempted to upload '{filename}' but fileStream was unavailable.");
                throw new BadImageFormatException($"Unable to read image: '{filename}'.");
            }

            fileStream.Position = 0;
            try
            {
                //await image.SaveAsJpegAsync(Path.Combine(AppContext.BaseDirectory, "images", filename));
                //if (fileType.StartsWith(FileTypes.Image.Self)) throw new BadImageFormatException("Unsupported file format.");
                var blobClient = await GetContainerClientAsync(containerName ?? AzureBlobStorage.Containers.Uploads);
                var blob = blobClient.GetBlobClient(filename);
                await blob.UploadAsync(fileStream, true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e.StackTrace);
                throw e;
            }
        }
    }
}