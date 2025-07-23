using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using CloudCanvas.Shared.Utilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
namespace CloudCanvas.Functions;

public class ExtractMetadata(ILogger<ExtractMetadata> logger, BlobStorageService blobSerivce, ServiceBusAdapter service)
{
    private readonly ILogger<ExtractMetadata> _logger = logger;
    private readonly IBlobStorageService _blobSerivce = blobSerivce;
    private readonly IServiceBusAdapter _sbAdapter = service;

    /// <summary>
    /// Processes a blob triggered by an upload event, extracts metadata, and sends a message to a Service Bus topic.
    /// </summary>
    /// <remarks>This method is triggered by a blob upload to the specified container. It extracts metadata
    /// from the blob, logs the processing details, and sends a message to the Service Bus topic for further
    /// processing.</remarks>
    /// <param name="input">The stream representing the uploaded blob content.</param>
    /// <param name="name">The name of the uploaded blob.</param>
    /// <returns>A <see cref="ServiceBusMessageDTO"/> containing the event details, subject, and extracted metadata.</returns>
    [Function(nameof(ExtractMetadata))]
    public async Task Run([BlobTrigger(BlobStorage.Containers.Uploads + "/{name}", Connection = Secrets.MNSTRG)] Stream input, string name)
    {
        Validate.StringValue(nameof(name), name); // No use moving forward withouth is name
        _logger.LogInformation("[START] Function Start - Extracting metadata from blob: {name}", name);
        const string uploads = BlobStorage.Containers.Uploads;
        var bcClient = await _blobSerivce.GetOrCreateContainerClientAsync(uploads);
        var blob = bcClient.GetBlobClient(name); // There is already validation on name etc from the SDK
        BlobMetaDTO metadata = CCSerializer.FromBlobProperties(name, blob.Uri.ToString(), blob.GetProperties());
        metadata.ContainerName = BlobStorage.Containers.Uploads;
        metadata.Name = name;
        metadata.ProcessingStage = (int) BlobProcessingStage.ExtractMetadata;
        metadata.Project = "CloudCanvas"; // TODO: This should be dynamic, based on the blob name or metadata
        _logger.LogInformation("[OK] Extracted Metadata from blob: {name}. Sending Message...", name);

        var message = MessageFactory.BuildFor(metadata) // Manual dispatch required for full control (dotnet-isolated)
            .WithSubject($"{ServiceBus.Status.MetadataExctracted} - file ready for processing.")    // Add Subject
            .AddProperty(ServiceBus.Props.EventType, ServiceBus.Subs.ExtractMetaData) // So that it makes it through subscription filters
            .AddProperty(ServiceBus.Props.ThumbnailSize, (int) ThumbnailSize.small) // BuildFor thumbnail generation, later used by orchestrators to fan-out differet sizes
            .SetCorrelationId(Guid.NewGuid().ToString()) // Set a new CorrelationId for this message, as the first in the chain
            .Finalize(); // Finalize and return the message ---> TODO: add custom (meaningful & descriptive) message ID builder, like {eventType}-{blobName}-{timestamp}
        var responseMessageId = await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, message); // Send the message and call it a day
        _logger.LogInformation("[DONE] Sent Message '{messageId}' to topic '{topic}': {subject}", responseMessageId, ServiceBus.Topics.FileUpdates, message.Subject);
    }
}