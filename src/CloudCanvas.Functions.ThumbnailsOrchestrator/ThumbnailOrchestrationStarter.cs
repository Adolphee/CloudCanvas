using Azure.Messaging.ServiceBus;
using CloudCanvas.Application.Posts.Photos;
using static CloudCanvas.Application.Common.Constants.ServiceBus;
namespace CloudCanvas.Functions.ThumbnailOrchestrator;

public class ThumbnailOrchestrationStarter
{
    [Function(nameof(ThumbnailOrchestrationStarter))]
    public async Task<string?> Run(
        [ServiceBusTrigger(Topics.FileUpdates, Subs.CreateThumbnail, Connection = Secrets.FUMSGI)]
        ServiceBusReceivedMessage incoming,
        ServiceBusMessageActions messageActions,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext, CancellationToken cancellation = default)
    {
        var logger = executionContext.GetLogger<ThumbnailOrchestrationStarter>();
        logger.LogInformation("{correlationId} Received thumbnail orchestration trigger", incoming.CorrelationId);
        
        try {
            var photo = incoming.Body.ToObjectFromJson<PhotoDTO>(); 
            var containerName = incoming.ApplicationProperties[Props.ContainerName]?.ToString()!;
            var request = new InceptionRequest(photo!, containerName, incoming.CorrelationId); // Forced correlation for App Insights
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ThumbnailOrchestrator), request, cancellation);
            return instanceId; 
        } catch (Exception e) when (e is CCMapperException || e is ArgumentNullException)
        {
            await messageActions.AbandonMessageAsync(incoming, default, cancellation);
            logger.LogInformation(e, "{correlationId} Failed to deserialize request payload. Message abandoned: {messageId}", incoming.CorrelationId, incoming.MessageId);
            return null;
        }
    }
}