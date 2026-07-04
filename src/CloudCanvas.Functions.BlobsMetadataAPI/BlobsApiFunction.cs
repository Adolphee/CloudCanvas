using ICosmos = CloudCanvas.Application.Abstractions.Cosmos.IPostsRepositoryCosmos<CloudCanvas.Domain.Posts.Contracts.IPost>;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser;

namespace CloudCanvas.Functions.Api.BlobStorageApi;

public class BlobsApiFunction
{
    private readonly ILogger<BlobsApiFunction> _logger;
    private readonly ICosmos _cosmos;
    private const string CNTNR = CloudCosmos.Containers.BlobMeta;


    public BlobsApiFunction(ILogger<BlobsApiFunction> logger, ICosmos client)
    {
        _logger = logger;
        _cosmos = client;
    }

    [Function(name: "GetAllPhotos")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "photos")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Incoming {red.Method} request at '{route}'. Using container '{container}'.", req.Method, req.RouteValues, CNTNR);
            var res = await new GetAllPhotosRequestHandler(_cosmos).Handle(new GetAllPhotosQuery());
            return new OkObjectResult(res);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get metadata. Method: {req.Method}, Message: {msg}", req.Method, e.Message);
            return new UnprocessableEntityObjectResult(new { error = e.Message });
        }
    }

    [Function(name: "GetUserPhotos")]
    public async Task<IActionResult> GetUserPhotos([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user-photos")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Incoming {red.Method} request at '{route}'. Using container '{container}'.", req.Method, req.RouteValues, CNTNR);
            if(req.Query.TryGetValue("usr", out var userId))
            {
                var res = await new GetUserPhotosRequestHandler(_cosmos).Handle(new GetUserPhotosQuery(userId!, CNTNR));
                return new OkObjectResult(res);
            }
            return new BadRequestObjectResult("No UserId speicified.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get metadata. Method: {req.Method}, Message: {msg}", req.Method, e.Message);
            return new UnprocessableEntityObjectResult(new { error = e.Message });
        }
    }
}