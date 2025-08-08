using Azure.Messaging.ServiceBus;
using CloudCanvas.Functions.Orchestration.DTO;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.Durable.Starters;

public class ThumbnailOrchestrationStarter
{

    [Function(nameof(ThumbnailOrchestrationStarter))]
    public async Task<string?> Run(
        [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.CreateThumbnail, Connection = Secrets.FUMSGI)]
        ServiceBusReceivedMessage incoming,
        ServiceBusMessageActions messageActions,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        var logger = executionContext.GetLogger<ThumbnailOrchestrationStarter>();
        logger.LogInformation("{correlationId} Received thumbnail orchestration trigger", incoming.CorrelationId);
        using var reader = new StreamReader(incoming.Body.ToStream());
        var payload = await reader.ReadToEndAsync();
        try
        {
            var blob = CCSerializer.Deserialize<BlobMetaDTO>(payload); // Validation behind the scenes
            var request = new InceptionRequest(blob, incoming.CorrelationId); // forced correlation for App Insights
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ThumbnailOrchestrator), request);
            return instanceId;
        }
        catch (Exception e) when (e is CCSerializationException || e is ArgumentNullException)
        {
            await messageActions.AbandonMessageAsync(incoming);
            logger.LogInformation(e, "{correlationId} Failed to deserialize request payload. Message abandoned: {messageId}", incoming.CorrelationId, incoming.MessageId);
            return null;
        }

    }
}