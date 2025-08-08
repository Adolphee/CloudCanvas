using CloudCanvas.Functions.Orchestration.DTO;
using CloudCanvas.Functions.Orchestration.DTOs;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.Orchestration.Activities;

public class PublishThumbnailCompletionActivity(ServiceBusAdapter serviceBusAdapter)
{
    private readonly ServiceBusAdapter _sbAdapter = serviceBusAdapter;

    [Function(nameof(PublishThumbnailCompletionActivity))]
    public async Task<BlobMetaDTO> Run([ActivityTrigger] RequestContext req, FunctionContext context)
    {
        var logger = context.GetLogger<PublishThumbnailCompletionActivity>();
        logger.LogInformation("{correlationId} Thumbnails orchestration complete. BlobId: {identifier}, InstanceId: {instanceId}", req.CorrelationId, req.Blob.Name, req.InstanceId);
        try
        {
            var SbNotification = MessageFactory.BuildFor(req.Blob)
                .WithSubject(ServiceBus.Status.OrchestrationFinished)
                .SetCorrelationId(req.CorrelationId)
                .AddProperty(BlobStorage.Meta.CompletedOn, DateTimeOffset.Now)
                .Finalize(req.InstanceId);

            await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, SbNotification);
            logger.LogInformation("{correlationId} Done. Sent Message '{messageId}' to topic '{topic}': {subject}", 
                SbNotification.CorrelationId, SbNotification.MessageId, ServiceBus.Topics.FileUpdates, SbNotification.Subject);
        }
        catch (Exception e) when (e is CCSerializationException || e is InvalidArgumentException)
        {
            logger.LogError(e, "{correlationId} Failed to deserialize metadata into an object of type {type}. Operation aborted. ", req.CorrelationId, nameof(BlobMetaDTO));
            throw;
        }
        return req.Blob;
    }
}