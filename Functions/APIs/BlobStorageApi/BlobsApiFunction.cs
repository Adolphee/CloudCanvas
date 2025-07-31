using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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
        const string _container = CloudCosmos.Containers.BlobMeta;
        _logger.LogInformation("[Http:{req.Method}] Incoming request at '/api/photos'... Fetching metadata from cosmos container '{container}'...", req.Method, _container);
        var res = await _cosmos.ListBlobsAsync<GalleryItemDTO>(_container);
        return new OkObjectResult(res);
    }

    [Function("SaveBlob")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post", "put", Route = "photos")] HttpRequest req)
    {
        _logger.LogInformation("[Http:{req.Method}] Incoming request at '{route}'... Verifying & Saving metadata...", req.Method, req.RouteValues);
        using var reader = new StreamReader(req.Body);
        var payload = await reader.ReadToEndAsync();
        bool overwrite = req.Method == "PUT";
        try
        {
            var meta = CCSerializer.Deserialize<GalleryItemDTO>(payload); // Validation behind the scenes
            var res = await _cosmos.SaveMetadataAsync(meta, CloudCosmos.Containers.BlobMeta, overwrite);
            _logger.LogInformation("[Http:{req.Method}] Successfully saved metadata for blob '{blobName}' to container . Signing off...", req.Method, meta.Id); // meta.Id always == blobName (or meta.Name)
            return new OkObjectResult(res);
        }
        catch (CCSerializationException e)
        {
            _logger.LogError(e, "[Http:{req.Method}] ERROR: Deserialization of incoming request.body failed with message:\n{msg}", req.Method, e.Message);
            return new UnprocessableEntityObjectResult(new { error = e.Message });
        }
    }

    [Function("PatchBlobMetadata")]
    public async Task<IActionResult> Patch([HttpTrigger(AuthorizationLevel.Function, "patch", Route = "photos/{identifier}")] HttpRequest req, string identifier)
    {
        _logger.LogInformation("[Http:{req.Method}] Request to patch blob/file '{name}'. Verifying data, then Applying changes...", req.Method, identifier);
        using var reader = new StreamReader(req.Body);
        var stream = await reader.ReadToEndAsync();
        try
        {
            var dto = CCSerializer.Deserialize<PatchGalleryItemDTO>(stream);
            var ops = PatchOperationBuilder.For(dto);
            dto = await _cosmos.PatchItemAsync(identifier, dto.UserId, ops);
            _logger.LogInformation("[Http:{req.Method}] Successfully processed patch for blob/file '{name}'. Signing off...", req.Method, identifier);
            return new ObjectResult(dto);
        } catch (Exception e) when (e is CosmosContainerNotFoundException || e is CCSerializationException)
        {
            _logger.LogError("[Http:{req.Method}] Failed to patch blob/file '{name}'.", req.Method, identifier);
            return null; // what should I return here?
        }
    }


    [Function("SoftDeleteBlob")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "photos/{name}")] HttpRequest req, string name)
    {
        _logger.LogInformation("[Http:{req.Method}] Soft-delete requested for blob/file '{name}'. Applying to cosmos and blob storage...", req.Method, name);
        var container = await _bservice.GetOrCreateContainerClientAsync(BlobStorage.Containers.Uploads, false);
        var bclient = container.GetBlobClient(name);
        var props = await bclient.GetPropertiesAsync();
        BlobMetaDTO blob = CCSerializer.MetaFromBlobProperties(name, bclient.Uri.ToString(), props);
        blob.DeletedOn = DateTimeOffset.Now;
        blob.Metadata[BlobStorage.Meta.DeletedOn] = blob.DeletedOn.ToString()!;
        try
        {
            blob = await _cosmos.SaveMetadataAsync(blob, CloudCosmos.Containers.BlobMeta, true);
            await bclient.SetMetadataAsync(blob.Metadata);
            var blob_delete_success = await _bservice.DeleteAsync(container.Name, name);
            _logger.LogInformation("[Http:{req.Method}] Soft-deletion carried out successfully: '{name}' deleted from {containerName} and {.", req.Method, name, container.Name);
            return new OkObjectResult(204);
        }
        catch (Exception e) when (e is InvalidArgumentException || e is ArgumentNullException)
        {
            _logger.LogWarning(e, "[Http:{req.Method}] Failed to delete blob '{name}' from container '{containerName}'.", req.Method, name, container.Name);
            return new NotFoundObjectResult($"Oops! This gallery item can no longer be found.");
        }
    }
}