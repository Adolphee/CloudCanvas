using CloudCanvas.Shared;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Runtime.CompilerServices;

namespace CloudCanvas.Functions.Api.BlobStorageApi;

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

    [Function(name: "GetAllBlobs")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "photos")] HttpRequest req)
    {
        try
        {
            const string _container = CloudCosmos.Containers.BlobMeta;
            _logger.LogInformation("Incoming {red.Method} request at '{route}'. Using container '{container}'.", req.Method, req.RouteValues, _container);
            var res = await _cosmos.ListBlobsAsync<GalleryItemDTO>(_container);
            return new OkObjectResult(res);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get metadata. Method: {req.Method}, Message: {msg}", req.Method, e.Message);
            return new UnprocessableEntityObjectResult(new { error = e.Message });
        }
    }


    [Function(name: "GetSingleBlob")]
    public async Task<IActionResult> GetSingle([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "photos/{identifier}")] HttpRequest req, string identifier, string userId)
    {
        try
        {
            const string _container = CloudCosmos.Containers.BlobMeta;
            _logger.LogInformation("Incoming {red.Method} request at '{route}'. Using container '{container}'.", req.Method, req.RouteValues, _container);
            var res = await _cosmos.SingleAsync<GalleryItemDTO>(identifier, userId, _container);
            return new OkObjectResult(res);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get metadata. Method: {req.Method}, Message: {msg}", req.Method, e.Message);
            return new UnprocessableEntityObjectResult(new { error = e.Message });
        }
    }

    [Function(name: "SaveBlob")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Anonymous, "post", "put", Route = "photos")] HttpRequest req)
    {
        _logger.LogInformation("Incoming {req.Method} Request at '{route}'.", req.Method, req.RouteValues);
        using var reader = new StreamReader(req.Body);
        var payload = await reader.ReadToEndAsync();
        bool overwrite = req.Method == "PUT";
        try
        {
            var meta = CCSerializer.Deserialize<GalleryItemDTO>(payload); // Validation behind the scenes
            var res = await _cosmos.SaveMetadataAsync(meta, CloudCosmos.Containers.BlobMeta, overwrite);
            _logger.LogInformation("Received {req.Method} Saved metadata for blob '{identifier}'.", req.Method, res.Id); // meta.Id always == identifier (or meta.Name)
            return new OkObjectResult(new { StatusCode = HttpStatusCode.Created, Value = res });
        }
        catch (CCSerializationException e)
        {
            _logger.LogError(e, "Deserialization of {req.Method} payload failed with message: {msg}", req.Method, e.Message);
            return new UnprocessableEntityObjectResult(new { error = e.Message });
        }
    }

    [Function(name: "PatchBlobMetadata")]
    public async Task<IActionResult> Patch([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "photos/{identifier}")] HttpRequest req, string identifier)
    {
        _logger.LogInformation("{req.Method} Request for blob '{identifier}'.", req.Method, identifier);
        using var reader = new StreamReader(req.Body); // I've serialized the DTO object in the req.body
        var stream = await reader.ReadToEndAsync();
        try
        {
            var dto = CCSerializer.Deserialize<PatchGalleryItemDTO>(stream);
            var ops = PatchOperationBuilder.For(dto);
            if (ops.Any())
            {
                string container = CloudCosmos.Containers.BlobMeta;
                dto = await _cosmos.PatchItemAsync<PatchGalleryItemDTO>(identifier, dto.UserId, container, ops);
                _logger.LogInformation("Carried out {operationCount} {req.Method} operations for blob '{containerName}/{identifier}'", ops.Count(), req.Method, container, identifier);
                return new OkObjectResult(dto);
            }
            _logger.LogInformation("Patch-list empty for '{identifier}'. {req.Method} Request ignored.", identifier,  req.Method);
            return new BadRequestObjectResult(new { StatusCode = HttpStatusCode.Created, Value = "Patch-list is empty. Please make/add at least one valid change." });
        } catch (Exception e) when (e is CosmosContainerNotFoundException || e is CCSerializationException)
        {
            _logger.LogError(e, "{req.Method} Request failed for blob '{identifier}'.", req.Method, identifier);
            return new BadRequestObjectResult(new { StatusCode = HttpStatusCode.Created, Value = "Failed to update this gallery item. Please refresh and try again." }); // what should I return here?
        }
    }


    [Function(name: "SoftDeleteBlob")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "photos/{identifier}")] HttpRequest req, string identifier)
    {
        _logger.LogInformation("Soft-delete requested. Method: {req.Method}, blobId: '{identifier}'", req.Method, identifier);
        BlobMetaDTO blob = await _bservice.GetBlobMetaAsync(identifier, BlobStorage.Containers.Uploads);
        blob.DeletedOn = DateTimeOffset.Now;
        blob = await _bservice.AddMetadataAsync(blob, BlobStorage.Meta.DeletedOn, blob.DeletedOn.ToString()!);
        using var reader = new StreamReader(req.Body);
        var bodyString = await reader.ReadToEndAsync();
        try
        {
            Validate.StringValue(nameof(bodyString), bodyString);
            // first, soft-delete metadata, if that works then do the physical blob too
            var patch = new PatchGalleryItemDTO
            {
                DeletedOn = blob.DeletedOn
            };
            var ops = PatchOperationBuilder.For(blob, true);
            var patchedItem = await _cosmos.PatchItemAsync<PatchGalleryItemDTO>(blob.Id, blob.UserId, CloudCosmos.Containers.BlobMeta, ops);
            var blob_delete_success = await _bservice.DeleteAsync(blob.ContainerName, identifier); // works regardless of blob's existence, but might throw on container 404
            _logger.LogInformation("Soft-deleted blob '{identifier}' from {containerName}", identifier, blob.ContainerName);
            return new OkObjectResult(new { StatusCode = 204, Value = "Not Found"});
        }
        catch (Exception e) when (e is CCSerializationException || e is InvalidArgumentException || e is ArgumentNullException)
        {
            _logger.LogWarning(e, "{req.Method} Request Failed: soft-delete blob '{identifier}' from container '{containerName}'", req.Method, identifier, blob.ContainerName);
            return new NotFoundObjectResult(new { StatusCode = 204, Value = "Not Found" });
        }
    }
}