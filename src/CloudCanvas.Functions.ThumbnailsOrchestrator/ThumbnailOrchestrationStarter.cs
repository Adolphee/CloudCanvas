using Azure.Messaging.ServiceBus;
using CloudCanvas.Application.Common;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Functions.ThumbnailOrchestrator.DTO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions.ThumbnailOrchestrator;

public class ThumbnailOrchestrationStarter
{
    [Function(nameof(ThumbnailOrchestrationStarter))]
    public async Task<string?> Run(
        [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.CreateThumbnail, Connection = ServiceBus.ManagedIdentity)]
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
            var photo = CCSerializer.Deserialize<PhotoDTO>(payload); // Validation behind the scenes
            var request = new InceptionRequest(photo, incoming.CorrelationId); // forced correlation for App Insights
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ThumbnailOrchestrator), request);
            return instanceId;
        } catch (Exception e) when (e is CCSerializationException || e is ArgumentNullException)
        {
            await messageActions.AbandonMessageAsync(incoming);
            logger.LogInformation(e, "{correlationId} Failed to deserialize request payload. Message abandoned: {messageId}", incoming.CorrelationId, incoming.MessageId);
            return null;
        }
    }
}