using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BlobStorageApi;

public class GetBlobsFunction
{
    private readonly ILogger<GetBlobsFunction> _logger;
    private readonly CosmosClientWrapper _cosmos;
    
    public GetBlobsFunction(ILogger<GetBlobsFunction> logger, CosmosClientWrapper cosmosWrapper)
    {
        _logger = logger;
        _cosmos = cosmosWrapper;
    }

    [Function(nameof(GetBlobsFunction))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        var res = await _cosmos.ListBlobsAsync(CloudCosmos.Containers.BlobMeta);
        return new OkObjectResult(res);
    }
}