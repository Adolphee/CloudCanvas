using Azure.Storage.Blobs;
using CloudCanvas.Constants;
using CloudCanvas.Interfaces;

namespace CloudCanvas.Services
{
    public class BlobStorageService: IBlobStorageService
    {
        private readonly IConfiguration _config;
        public BlobStorageService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<string>> GetUrlsAsync()
        {
            var cfg = _config.GetSection(AzureBlobStorage.Self);
            var cstring = cfg[AzureBlobStorage.ConnectionString];
            var cname = cfg[AzureBlobStorage.ContainerName];
            var blobClient = new BlobContainerClient(cstring, cname);


            var items = new List<string>();
            await foreach (var item in blobClient.GetBlobsAsync())
            {
                var blob = blobClient.GetBlobClient(item.Name);
                items.Add(blob.Uri.ToString());
            }
            return items;
        }

        public async Task UploadAsync(IFormFile formFile)
        {
            if (formFile == null) throw new BadImageFormatException("No image provided.");
            
            var cfg = _config.GetSection(AzureBlobStorage.Self);
            var cstring = cfg[AzureBlobStorage.ConnectionString];
            var cname = cfg[AzureBlobStorage.ContainerName];
            var blobClient = new BlobContainerClient(cstring, cname);
            await blobClient.CreateIfNotExistsAsync();

            var blob = blobClient.GetBlobClient(formFile.FileName);
            using var stream = formFile.OpenReadStream();
            await blob.UploadAsync(stream, true);
        }
    }
}