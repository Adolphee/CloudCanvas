using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Functions.ThumbnailOrchestrator.Activities;
using Microsoft.DurableTask;

namespace CloudCanvas.Functions.ThumbnailOrchestrator;

public class ThumbnailOrchestrator
{
    [Function(nameof(ThumbnailOrchestrator))]
    public static async Task<PhotoDTO> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext orchest_ctx, InceptionRequest req, RequestContext context)
    {   // log the invocation of the orchestrator
        var logger = orchest_ctx.CreateReplaySafeLogger(nameof(ThumbnailOrchestrator));
        logger.LogInformation("{correlationId} Thumbnail orchestration invoked. instanceId: {instanceId}, blobId: {identifier}",
            req.CorrelationId, orchest_ctx.InstanceId, req.Photo.Id);

        // initial/future state
        var thumbnails = new Dictionary<ThumbnailSize, Task<string>>();

        // fan-out
        foreach (var size in Enum.GetValues<ThumbnailSize>()) 
        {
            var thumb_req = new CreateThumbnailActivityRequest(req.Photo, size, req.SrcContainer, req.CorrelationId, orchest_ctx.InstanceId);
            thumbnails.Add(size, orchest_ctx.CallActivityAsync<string>(nameof(CreateThumbnailActivity), thumb_req));
        }

        // fan-in
        var results = await Task.WhenAll(thumbnails.Values); 
        logger.LogInformation("{correlationId} Fan-out/Fan-in completed. instanceId: {instanceId}, blobId: {identifier}",
            req.CorrelationId, orchest_ctx.InstanceId, req.Photo.Id);

        // add thumbnails
        foreach ((var size, var url) in thumbnails) 
        {
            req.Photo.Thumbnails.Add(size.ToString(), await url);
        }

        // save result to CosmosDB
        var meta_req = new SaveThumbnailsActivityRequest(req.Photo, req.SrcContainer, req.CorrelationId, orchest_ctx.InstanceId);
        var finalMetadata = await orchest_ctx.CallActivityAsync<PhotoDTO>(nameof(SaveThumbnailsActivity), meta_req);

        // publish results to service bus
        var pub_req = new PublishThumbnailCompletionRequest(req.Photo, req.CorrelationId, orchest_ctx.InstanceId);
        await orchest_ctx.CallActivityAsync(nameof(PublishThumbnailCompletionActivity), pub_req);
        
        return finalMetadata;
    }
}