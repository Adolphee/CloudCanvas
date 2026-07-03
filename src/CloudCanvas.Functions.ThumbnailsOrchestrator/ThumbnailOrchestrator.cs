using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Functions.Orchestration.Activities;
using CloudCanvas.Functions.Orchestration.DTO;
using CloudCanvas.Infrastructure.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.Durable.Orchestrators;

public class ThumbnailOrchestrator
{
    [Function(nameof(ThumbnailOrchestrator))]
    public static async Task<BlobMetadata> RunOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context, InceptionRequest req)
    {
        var logger = context.CreateReplaySafeLogger(nameof(ThumbnailOrchestrator));
        logger.LogInformation("{correlationId} Thumbnail orchestration invoked. instanceId: {instanceId}, blobId: {identifier}",
            req.CorrelationId, context.InstanceId, req.Blob.Name);
        var thumbnails = new Dictionary<ThumbnailSize, Task<string>>();
        var sizes = new[] { ThumbnailSize.xsmall, ThumbnailSize.small, ThumbnailSize.medium };
        var thumb_req = new ThumbnailActivityRequest(req.Blob, req.CorrelationId, context.InstanceId);

        // fan-out
        foreach (var tsize in sizes) 
        {
            thumb_req.ThumbnailSize = tsize;
            thumbnails[tsize] = context.CallActivityAsync<string>(nameof(CreateThumbnailActivity), thumb_req);
        }
        // fan-in
        var results = await Task.WhenAll(thumbnails.Values); 
        logger.LogInformation("{correlationId} Fan-out/Fan-in completed. instanceId: {instanceId}, blobId: {identifier}",
            req.CorrelationId, context.InstanceId, req.Blob.Name);

        // add thumbnails
        foreach ((var size, var url) in thumbnails) 
        {
            req.Blob.Thumbnails.Add(size, url.Result); // url.Result is safe here: already awaited Task.WhenAll
        }
        // save result to CosmosDB
        var meta_req = new PersistMetadataActivityRequest(req.Blob, req.CorrelationId, context.InstanceId);
        var finalMetadata = await context.CallActivityAsync<BlobMetadata>(nameof(PersistMetadataActivity), meta_req);

        // publish results to service bus
        var pub_req = new PublishCompletionRequest(req.Blob, req.CorrelationId, context.InstanceId);
        await context.CallActivityAsync(nameof(PublishThumbnailCompletionActivity), pub_req);
        
        return finalMetadata;
    }
}