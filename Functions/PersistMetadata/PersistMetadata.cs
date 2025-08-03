using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Shared.Exceptions;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Functions;

/// <summary>
/// Extracts Metadata from uploaded blobs, received as DTO objects through Service Bus Messages
/// </summary>

public class PersistMetadata(ILogger<PersistMetadata> logger, ServiceBusAdapter serviceBusAdapter, CosmosClientWrapper cosmos)
{
    private readonly ILogger<PersistMetadata> _logger = logger;
    private readonly IServiceBusAdapter _sbAdapter = serviceBusAdapter;
    private readonly ICosmosClientWrapper _cosmos = cosmos;
    private readonly int _maxMessageLength = Convert.ToInt32(Environment.GetEnvironmentVariable(Config.MaxMessageLength)); // Enables configurable max message size

    /// <summary>
    /// Processes a Service Bus message containing metadata, persists the metadata, and returns a response message.
    /// </summary>
    /// <remarks>This method is triggered by a Service Bus message and is configured to output a message to
    /// the specified Service Bus topic. It processes the metadata, persists it (e.g., to a database), and constructs a
    /// response message indicating the processing status.</remarks>
    /// <param name="incoming">The received Service Bus message containing metadata to be processed.</param>
    /// <param name="messageActions">Provides actions that can be performed on the Service Bus message, such as completing or abandoning it.</param>
    /// <returns>A <see cref="ServiceBusMessageDTO"/> containing information about the completion of metadata processing.</returns>
    [Function(nameof(PersistMetadata))]
    public async Task Run(
        [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.PersistMetadata, Connection = Secrets.FUMSGI)]
        ServiceBusReceivedMessage incoming,
        ServiceBusMessageActions messageActions)
    {
        if(!await ValidateReceivedMessage(incoming, messageActions)) return;

        try
        {
            BlobMetaDTO metadata = CCSerializer.MetaFromBinaryData<BlobMetaDTO>(incoming.Body);      // Validate & Deserialize body
            metadata.ProcessingStage = (int) BlobProcessingStage.UpdateMetadata;
            metadata.LastModified = DateTimeOffset.Now;
            metadata = await _cosmos.SaveMetadataAsync(metadata, CloudCosmos.Containers.BlobMeta, true);      // Overwrite metadata to CosmosDB
            _logger.LogInformation("{correlationId} Metadata Persisted for blob {identifier}", incoming.CorrelationId, metadata.Id);
            
            var response = MessageFactory.BuildFor(metadata)                                    // Manual dispatch required for full control (dotnet-isolated)
                .WithSubject($"{ServiceBus.Status.MetadataPersisted} - file ready for further processing.")         // Add Subject
                .AddProperty(ServiceBus.Props.EventType, ServiceBus.Subs.PersistMetadata)       // So that it makes it through subscription filters
                .Finalize(incoming.CorrelationId);                                               // Finalize and return the message
            await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, response);               // Send the message and call it a day
            _logger.LogInformation("{correlationId} Done. Sent Message '{messageId}' to topic '{topic}': {subject}", response.CorrelationId, response.MessageId, ServiceBus.Topics.FileUpdates, response.Subject);
        } catch (Exception e) when (e is CCSerializationException  || e is InvalidArgumentException)
        {
            await messageActions.AbandonMessageAsync(incoming); // just for now implement retry & orchestration later, DLQ on attempt X
            _logger.LogError(e, "{correlationId} Failed to deserialize metadata into an object of type {type}. Operation aborted. ", incoming.CorrelationId, nameof(BlobMetaDTO));
        }
    }

    private async Task<bool> ValidateReceivedMessage(ServiceBusReceivedMessage incoming, ServiceBusMessageActions messageActions)
    {
        try { Validate.SBMessageSize(incoming, _maxMessageLength); }
        catch (MessageTooLargeException e) // Unexpectedly large? considered unsafe, discard
        {
            _logger.LogWarning(e, "{correlationId} Message '{messageId}' too large. Size: {messageLength}/{maxMessageLength} Bytes. DLQ Material -> Skipping...", incoming.CorrelationId, incoming.MessageId, e.ActualMessageSize, e.MaxMessageSize);
            await messageActions.DeadLetterMessageAsync(incoming, deadLetterReason: nameof(MessageTooLargeException), deadLetterErrorDescription: e.Message); // Skip and throw awway
            return false;
        }
        return true;
    }
}