using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Functions.ThumbnailOrchestrator.Activities;
using CloudCanvas.Functions.ThumbnailOrchestrator.DTO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.ThumbnailOrchestrator;

public class ThumbnailOrchestrator
{
    [Function(nameof(ThumbnailOrchestrator))]
    public static async Task<PhotoDTO> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context, InceptionRequest req)
    {   // log the invocation of the orchestrator
        var logger = context.CreateReplaySafeLogger(nameof(ThumbnailOrchestrator));
        logger.LogInformation("{correlationId} Thumbnail orchestration invoked. instanceId: {instanceId}, blobId: {identifier}",
            req.CorrelationId, context.InstanceId, req.Photo.Id);

        // initial/future state
        var thumbnails = new Dictionary<ThumbnailSize, Task<string>>();

        // fan-out
        foreach (var size in Enum.GetValues<ThumbnailSize>()) 
        {
            var thumb_req = new CreateThumbnailActivityRequest(req.Photo, size, req.CorrelationId, context.InstanceId);
            thumbnails.Add(size, context.CallActivityAsync<string>(nameof(CreateThumbnailActivity), thumb_req));
        }

        // fan-in
        var results = await Task.WhenAll(thumbnails.Values); 
        logger.LogInformation("{correlationId} Fan-out/Fan-in completed. instanceId: {instanceId}, blobId: {identifier}",
            req.CorrelationId, context.InstanceId, req.Photo.Id);

        // add thumbnails
        foreach ((var size, var url) in thumbnails) 
        {
            req.Photo.Thumbnails.Add(size.ToString(), await url);
        }

        // save result to CosmosDB
        var meta_req = new SaveThumbnailsActivityRequest(req.Photo, req.CorrelationId, context.InstanceId);
        var finalMetadata = await context.CallActivityAsync<PhotoDTO>(nameof(SaveThumbnailActivity), meta_req);

        // publish results to service bus
        var pub_req = new PublishCompletionRequest(req.Photo, req.CorrelationId, context.InstanceId);
        await context.CallActivityAsync(nameof(PublishThumbnailCompletionActivity), pub_req);
        
        return finalMetadata;
    }
}