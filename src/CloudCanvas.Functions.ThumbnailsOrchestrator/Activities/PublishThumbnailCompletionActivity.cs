using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Common.Mapping;
using static CloudCanvas.Application.Common.Constants.ServiceBus;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.Activities;

public class PublishThumbnailCompletionActivity(IMessenger messenger, ILogger<PublishThumbnailCompletionActivity> logger)
{
    private readonly IMessenger _messanger = messenger;
    private readonly ILogger<PublishThumbnailCompletionActivity> _logger = logger;

    [Function(nameof(PublishThumbnailCompletionActivity))]
    public async Task Run([ActivityTrigger] PublishThumbnailCompletionRequest req, CancellationToken cancellation = default)
    {
        _logger.LogInformation("{correlationId} Thumbnails orchestration complete. BlobId: {identifier}, InstanceId: {instanceId}", req.CorrelationId, req.Photo.Id, req.InstanceId);
        try
        {
            await _messanger.SendCreateThumbnailsCompletionAsync(req.Photo, req.CorrelationId, cancellation);
            await  _messanger.NotifyReadyForIntelligenceAsync(req.Photo.ToEnrichmentTarget(), req.CorrelationId, cancellation);
        }
        catch (Exception e) when (e is CCMapperException || e is InvalidArgumentException)
        {
            _logger.LogError(e, "{correlationId} Failed to send Message to '{topic}': {subject}",
                req.CorrelationId, Topics.FileUpdates, Status.OrchestrationFinished);
            throw;
        }
    }
}