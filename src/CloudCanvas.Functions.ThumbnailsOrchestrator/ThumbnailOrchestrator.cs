using CloudCanvas.Domain.Enums;
using CloudCanvas.Functions.ThumbnailOrchestrator.Activities;
using Microsoft.DurableTask;

namespace CloudCanvas.Functions.ThumbnailOrchestrator;

public class ThumbnailOrchestrator
{
    [Function(nameof(ThumbnailOrchestrator))]
    public static async Task<PhotoDTO> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context, InceptionRequest req)
    {   // log the invocation of the orchestrator
        var logger = context.CreateReplaySafeLogger(nameof(ThumbnailOrchestrator));
        logger.LogInformation("{correlationId} Thumbnail orchestration invoked. instanceId: {instanceId}, blobId: {identifier}",
            req.correlationId, context.InstanceId, req.photo.Id);

        // initial/future state
        var thumbnails = new Dictionary<ThumbnailSize, Task<string>>();

        // fan-out
        foreach (var size in Enum.GetValues<ThumbnailSize>()) 
        {
            var thumb_req = new CreateThumbnailActivityRequest(req.photo, size, req.srcContainer, req.correlationId, context.InstanceId);
            thumbnails.Add(size, context.CallActivityAsync<string>(nameof(CreateThumbnailActivity), thumb_req));
        }

        // fan-in
        var results = await Task.WhenAll(thumbnails.Values); 
        logger.LogInformation("{correlationId} Fan-out/Fan-in completed. instanceId: {instanceId}, blobId: {identifier}",
            req.correlationId, context.InstanceId, req.photo.Id);

        // add thumbnails
        foreach ((var size, var url) in thumbnails) 
        {
            req.photo.Thumbnails.Add(size.ToString(), await url);
        }

        // save result to CosmosDB
        var meta_req = new SaveThumbnailsActivityRequest(req.photo, req.srcContainer, req.correlationId, context.InstanceId);
        var finalMetadata = await context.CallActivityAsync<PhotoDTO>(nameof(SaveThumbnailsActivity), meta_req);

        // publish results to service bus
        var pub_req = new PublishCompletionRequest(req.photo, req.correlationId, context.InstanceId);
        await context.CallActivityAsync(nameof(PublishThumbnailCompletionActivity), pub_req);
        
        return finalMetadata;
    }
}