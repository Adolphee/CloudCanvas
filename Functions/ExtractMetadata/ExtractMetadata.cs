using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Grpc.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
namespace CloudCanvas.Functions;

public class ExtractMetadata(ILogger<ExtractMetadata> logger, BlobStorageService blobSerivce, ServiceBusAdapter service, CosmosClientWrapper cosmos)
{
    private readonly ILogger<ExtractMetadata> _logger = logger;
    private readonly BlobStorageService _blobSerivce = blobSerivce;
    private readonly ServiceBusAdapter _sbAdapter = service;
    private readonly CosmosClientWrapper _cosmos = cosmos;

    /// <summary>
    /// Processes a blob triggered by an upload event, extracts metadata, and sends a message to a Service Bus topic.
    /// </summary>
    /// <remarks>This method is triggered by a blob upload to the specified container. It extracts metadata
    /// from the blob, logs the processing details, and sends a message to the Service Bus topic for further
    /// processing.</remarks>
    /// <param identifier="input">The stream representing the uploaded blob content.</param>
    /// <param identifier="identifier">The identifier of the uploaded blob.</param>
    /// <returns>A <see cref="ServiceBusMessageDTO"/> containing the event details, subject, and extracted metadata.</returns>
    [Function(nameof(ExtractMetadata))]
    public async Task Run([BlobTrigger(BlobStorage.Containers.Uploads + "/{identifier}", Connection = BlobStorage.BSConnection)] Stream input, string identifier)
    {
        string correlationId = Guid.NewGuid().ToString();
        const string uploads = BlobStorage.Containers.Uploads;
        _logger.LogInformation("{correlationId} Function Start - Extracting metadata from blob: {container}/{identifier}", correlationId, uploads, identifier);
        
        var bcClient = await _blobSerivce.GetOrCreateContainerClientAsync(uploads);
        var blob = bcClient.GetBlobClient(identifier); // There is already validation on identifier etc from the SDK
        try
        {
            BlobMetaDTO metadata = CCSerializer.MetaFromBlobProperties(identifier, blob.Uri.ToString(), blob.GetProperties());
            if (!await _cosmos.MetaExistsAsync(CloudCosmos.Containers.BlobMeta, metadata.Id, metadata.UserId))
            { // usually this function only executes ONCE; when the main file is uploaded.
                metadata = await _cosmos.SaveMetadataAsync(metadata, CloudCosmos.Containers.BlobMeta, false);
            }
            var message = BuildSBMessage(metadata, correlationId);
            var responseMessageId = await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, message); // Send the message and call it a day
            _logger.LogInformation("{correlationId} Metadata Extracted. Sent Message '{messageId}' to topic '{topic}': {subject}", message.CorrelationId, responseMessageId, ServiceBus.Topics.FileUpdates, message.Subject);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{correlationId} Failed to extract metadata from blob: {container}/{identifier}", correlationId, uploads, identifier);
            throw;
        }
    }

    private ServiceBusMessage BuildSBMessage(BlobMetaDTO metadata, string correlationId)
    {
        return MessageFactory.BuildFor(metadata) // Manual dispatch required for full control (dotnet-isolated)
            .WithSubject($"{ServiceBus.Status.NewBlobDetected} - Ready for processing")    // Add Subject
            .AddProperty(ServiceBus.Props.EventType, ServiceBus.Subs.ExtractMetaData) // So that it makes it through subscription filters
            .AddProperty(ServiceBus.Props.ThumbnailSize, (int)ThumbnailSize.small) // BuildFor thumbnail generation, later used by orchestrators to fan-out differet sizes
            .SetCorrelationId(correlationId) // Set a new CorrelationId for this message, as the first in the chain
            .Finalize(); // Finalize and return the message ---> bonus: add custom (meaningful & descriptive) message ID builder, like {eventType}-{blobName}-{timestamp}
    }
}