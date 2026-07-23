using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Functions.ThumbnailOrchestrator.DTO;
using CloudCanvas.Infrastructure.DTOs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.Activities;

public class PublishThumbnailCompletionActivity(IMessenger messenger)
{
    private readonly IMessenger _messanger = messenger;

    [Function(nameof(PublishThumbnailCompletionActivity))]
    public async Task<PhotoDTO> Run([ActivityTrigger] RequestContext req, FunctionContext context, CancellationToken cancellation = default)
    {
        var logger = context.GetLogger<PublishThumbnailCompletionActivity>();
        logger.LogInformation("{correlationId} Thumbnails orchestration complete. BlobId: {identifier}, InstanceId: {instanceId}", req.CorrelationId, req.Photo.Id, req.InstanceId);
        try
        {
            var res = await _messanger.NofityProjectionCompletedAsync(req.Photo, req.CorrelationId);
            logger.LogInformation("{correlationId} Sent Message '{messageId}' to topic '{topic}': {subject}", 
                req.CorrelationId, res, ServiceBus.Topics.FileUpdates, ServiceBus.Status.OrchestrationFinished);
        }
        catch (Exception e) when (e is CCSerializationException || e is InvalidArgumentException)
        {
            logger.LogError(e, "{correlationId} Failed to deserialize metadata into an object of type {type}. Operation aborted. ", req.CorrelationId, nameof(BlobMetadata));
            throw;
        }
        return req.Photo;
    }
}