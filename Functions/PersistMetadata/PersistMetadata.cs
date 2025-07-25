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
    /// <param name="message">The received Service Bus message containing metadata to be processed.</param>
    /// <param name="messageActions">Provides actions that can be performed on the Service Bus message, such as completing or abandoning it.</param>
    /// <returns>A <see cref="ServiceBusMessageDTO"/> containing information about the completion of metadata processing.</returns>
    [Function(nameof(PersistMetadata))]
    public async Task Run(
        [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.PersistMetadata, Connection = Secrets.FUMSGI)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("[{correlationId}][START] Function {functionName} triggered by {messageId}. Getting to work...", message.CorrelationId, nameof(PersistMetadata), message.MessageId);
        try { Validate.SBMessageSize(message, _maxMessageLength); }                             // try Validate message size
        catch (MessageTooLargeException e)                                                      // I'll let anything else 
        {
            _logger.LogWarning(e, "[{correlationId}][SKIP] Message '{messageId}' too large. Size: {messageLength}/{maxMessageLength} Bytes. DLQ Material ->  Skipping...", message.CorrelationId, message.MessageId, e.ActualMessageSize, e.MaxMessageSize);
            // If the message is larger than expected, it's considered unsafe... and thrown away. 
            await messageActions.DeadLetterMessageAsync(message, deadLetterReason: nameof(MessageTooLargeException), deadLetterErrorDescription: e.Message);
            return;
        }

        try
        {
            _logger.LogInformation("[{correlationId}] Validating payload...", message.CorrelationId);
            BlobMetaDTO metadata = CCSerializer.MetaFromBinaryData<BlobMetaDTO>(message.Body);      // Validate & Deserialize body
            metadata.ProcessingStage = (int) BlobProcessingStage.UpdateMetadata;
            _logger.LogInformation("[{correlationId}] Updating metadata for document '{documentId}'...", message.CorrelationId, metadata.Id);
            metadata = await _cosmos.SaveMetadataAsync(metadata, CloudCosmos.Containers.BlobMeta);      // Push metadata to CosmosDB
            var response = MessageFactory.BuildFor(metadata)                                    // Manual dispatch required for full control (dotnet-isolated)
                .WithSubject("Metadata Persisted - file ready for further processing.")         // Add Subject
                .AddProperty(ServiceBus.Props.EventType, ServiceBus.Subs.PersistMetadata)       // So that it makes it through subscription filters
                .Finalize(message.CorrelationId);                                               // Finalize and return the message
            await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, response);               // Send the message and call it a day
            _logger.LogInformation("[{correlationId}][DONE] Sent Message '{messageId}' to topic '{topic}': {subject}", response.CorrelationId, response.MessageId, ServiceBus.Topics.FileUpdates, response.Subject);
        } catch (CCSerializationException e)
        {
            _logger.LogError(e, "[{correlationId}][ERROR] Failed to deserialize metadata into an object of type {type}. Operation aborted. ", message.CorrelationId, nameof(BlobMetaDTO));
            throw;
        }
    }
}