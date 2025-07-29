using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BlobStorageApi;

public class BlobsApiFunction
{
    private readonly ILogger<BlobsApiFunction> _logger;
    private readonly ICosmosClientWrapper _cosmos;
    private readonly IBlobStorageService _bservice;
    
    public BlobsApiFunction(ILogger<BlobsApiFunction> logger, CosmosClientWrapper cosmosWrapper, BlobStorageService blobStorageService)
    {
        _logger = logger;
        _cosmos = cosmosWrapper;
        _bservice = blobStorageService;
    }

    [Function("ListBlobs")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "photos")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var res = await _cosmos.ListBlobsAsync<GalleryItemDTO>(CloudCosmos.Containers.BlobMeta);
        return new OkObjectResult(res);
    }

    [Function("CreateBlob")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", Route = "photos")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        using var reader = new StreamReader(req.Body);
        var payload = await reader.ReadToEndAsync();

        try
        {
            var meta = CCSerializer.Deserialize<BlobMetaDTO>(payload); // Validation behind the scenes
            var res = await _cosmos.SaveMetadataAsync(meta, CloudCosmos.Containers.BlobMeta);
            return new OkObjectResult(res);
        }
        catch (CCSerializationException e)
        {
            _logger.LogError(e, "[POST] ERROR: Serialization of incoming request.body failed with message:\n{msg}", e.Message);
            return new UnprocessableEntityObjectResult(new { error = e.Message });
        }
    }

    [Function("DeleteBlob")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "photos/{name}")] HttpRequest req, string name)
    {
        _logger.LogInformation("[Http:DELETE] Received HTTP request to delete blob/file '{name}'.", name);
        var container = await _bservice.GetOrCreateContainerClientAsync(BlobStorage.Containers.Uploads, false);
        var bclient = container.GetBlobClient(name);
        var props = await bclient.GetPropertiesAsync();
        BlobMetaDTO blob = CCSerializer.MetaFromBlobProperties(name, bclient.Uri.ToString(), props);
        blob.DeletedOn = DateTimeOffset.Now;
        blob.Metadata[BlobStorage.Meta.DeletedOn] = blob.DeletedOn.ToString()!;
        try
        {
            _logger.LogInformation("[Http:DELETE] Soft-deleting metadata '{name}' from container '{containerName}'...", name, container.Name);
            blob = await _cosmos.SaveMetadataAsync(blob, CloudCosmos.Containers.BlobMeta, true);
            _logger.LogInformation("[Http:DELETE] Soft-deleting blob file '{name}' from container '{containerName}'...", name, BlobStorage.Containers.Uploads);
            await bclient.SetMetadataAsync(blob.Metadata);
            var blob_delete_success = await _bservice.DeleteAsync(BlobStorage.Containers.Uploads, name);
            _logger.LogInformation("[Http:DELETE] Soft deletion request carried out successfully: '{name}' deleted from {containerName}.", name, container.Name);
            return new OkObjectResult(204);
        }
        catch (Exception e) when (e is InvalidArgumentException || e is ArgumentNullException)
        {
            _logger.LogWarning(e, "[Http:DELETE] Failed to delete blob '{name}' from container '{containerName}'.", name, container.Name);
            return new NotFoundObjectResult($"Oops! This gallery item can no longer be found.");
        }
    }
}