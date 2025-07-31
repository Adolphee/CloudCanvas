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
            _logger.LogInformation("[Http:{req.Method}] Successfully saved metadata for blob '{identifier}' to container . Signing off...", req.Method, res.Id); // meta.Id always == identifier (or meta.Name)
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
        _logger.LogInformation("[Http:{req.Method}] Request to patch blob/file '{identifier}'. Verifying data, then Applying changes...", req.Method, identifier);
        using var reader = new StreamReader(req.Body);
        var stream = await reader.ReadToEndAsync();
        try
        {
            var dto = CCSerializer.Deserialize<PatchGalleryItemDTO>(stream);
            var ops = PatchOperationBuilder.For(dto);
            if (ops.Any())
            {
                dto = await _cosmos.PatchItemAsync(identifier, dto.UserId, ops);
                _logger.LogInformation("[Http:{req.Method}] Successfully processed patch for blob/file '{identifier}'. Signing off...", req.Method, identifier);
                return new ObjectResult(dto);
            }
            _logger.LogInformation("[Http:{req.Method}] Change-list empty for '{identifier}'. False alarm, going back to sleep.", req.Method, identifier);
            return new BadRequestObjectResult("Change list is empty.");
        } catch (Exception e) when (e is CosmosContainerNotFoundException || e is CCSerializationException)
        {
            _logger.LogError("[Http:{req.Method}] Failed to patch blob/file '{identifier}'.", req.Method, identifier);
            return new BadRequestObjectResult("Failed to update this gallery item. Please refresh and try again."); // what should I return here?
        }
    }


    [Function("SoftDeleteBlob")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "photos/{identifier}")] HttpRequest req, string identifier)
    {
        _logger.LogInformation("[Http:{req.Method}] Soft-delete requested for blob/file '{identifier}'. Applying to cosmos and blob storage...", req.Method, identifier);
        var container = await _bservice.GetOrCreateContainerClientAsync(BlobStorage.Containers.Uploads, false);
        var bclient = container.GetBlobClient(identifier);
        var props = await bclient.GetPropertiesAsync();
        BlobMetaDTO blob = CCSerializer.MetaFromBlobProperties(identifier, bclient.Uri.ToString(), props);
        blob.DeletedOn = DateTimeOffset.Now;
        blob.Metadata[BlobStorage.Meta.DeletedOn] = blob.DeletedOn.ToString()!;
        try
        {
            blob = await _cosmos.SaveMetadataAsync(blob, CloudCosmos.Containers.BlobMeta, true);
            await bclient.SetMetadataAsync(blob.Metadata);
            var blob_delete_success = await _bservice.DeleteAsync(container.Name, identifier);
            _logger.LogInformation("[Http:{req.Method}] Soft-deletion carried out successfully: '{identifier}' deleted from {containerName} and {.", req.Method, identifier, container.Name);
            return new OkObjectResult(204);
        }
        catch (Exception e) when (e is InvalidArgumentException || e is ArgumentNullException)
        {
            _logger.LogWarning(e, "[Http:{req.Method}] Failed to delete blob '{identifier}' from container '{containerName}'.", req.Method, identifier, container.Name);
            return new NotFoundObjectResult($"Oops! This gallery item can no longer be found.");
        }
    }
}