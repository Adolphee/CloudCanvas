using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlobStorageApi;

public class BlobStorageApi
{
    private readonly ILogger<BlobStorageApi> _logger;
    private readonly CosmosClientWrapper _cosmos;
    
    public BlobStorageApi(ILogger<BlobStorageApi> logger, CosmosClientWrapper cosmosWrapper)
    {
        _logger = logger;
        _cosmos = cosmosWrapper;
    }

    [Function("GetBlobs")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var res = await _cosmos.ListBlobsAsync<GalleryItemDTO>(CloudCosmos.Containers.BlobMeta);
        return new OkObjectResult(res);
    }

    [Function("CreateBlob")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var payload = req.Query["image"].ToString();
        try
        {
            var meta = CCSerializer.Deserialize<GalleryItemDTO>(payload); // Validation behind the scenes
            var res = await _cosmos.SaveMetadataAsync(meta, CloudCosmos.Containers.BlobMeta);
            return new OkObjectResult(res);
        }
        catch (CCSerializationException e)
        {
            _logger.LogError(e, "[POST] ERROR: Serialization of incoming request.body failed with message:\n{msg}", e.Message);
            return new UnprocessableEntityObjectResult(new { error = e.Message });
        }
    }

}