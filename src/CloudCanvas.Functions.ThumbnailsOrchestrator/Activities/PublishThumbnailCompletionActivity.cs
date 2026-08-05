using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Posts.Photos;
using static CloudCanvas.Application.Common.Constants.ServiceBus;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.Activities;

public class PublishThumbnailCompletionActivity(IMessenger messenger, ILogger<PublishThumbnailCompletionActivity> logger)
{
    private readonly IMessenger _messanger = messenger;
    private readonly ILogger<PublishThumbnailCompletionActivity> _logger = logger;

    [Function(nameof(PublishThumbnailCompletionActivity))]
    public async Task<PhotoDTO> Run([ActivityTrigger] RequestContext req, CancellationToken cancellation = default)
    {
        _logger.LogInformation("{correlationId} Thumbnails orchestration complete. BlobId: {identifier}, InstanceId: {instanceId}", req.CorrelationId, req.Photo.Id, req.InstanceId);
        try
        {
            var res = await _messanger.SendCreateThumbnailsCompletionMessage(req.Photo, req.CorrelationId, cancellation);
            _logger.LogInformation("{correlationId} Sent Message '{messageId}' to topic '{topic}': {subject}", 
                req.CorrelationId, res, Topics.FileUpdates, Status.OrchestrationFinished);
        }
        catch (Exception e) when (e is CCMapperException || e is InvalidArgumentException)
        {
            _logger.LogError(e, "{correlationId} Failed to send Message to '{topic}': {subject}",
                req.CorrelationId, Topics.FileUpdates, Status.OrchestrationFinished);
            throw;
        }
        return req.Photo;
    }
}