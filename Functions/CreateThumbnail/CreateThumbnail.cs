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
public class CreateThumbnail(ILogger<CreateThumbnail> logger, BlobStorageService blobService, ServiceBusAdapter adapter)
{
    private readonly ILogger<CreateThumbnail> _logger = logger;
    private readonly IBlobStorageService _blobService = blobService;
    private readonly IServiceBusAdapter _sbAdapter = adapter;
    private readonly int _maxMessageLength = Convert.ToInt32(Environment.GetEnvironmentVariable(Config.MaxMessageLength)); // Enables configurable max message size

    /// <summary>
    /// Processes a incoming Service Bus  message to create a thumbnail image from the provided metadata and uploads it to a
    /// specified blob storage container.
    /// </summary>
    /// <remarks>This function listens to the Service Bus topic <see cref="ServiceBus.Topics.FileUpdates"/>
    /// and processes messages from the subscription <see cref="ServiceBus.Subs.CreateThumbnail"/>. It resizes the image
    /// specified in the metadata to a 50x50 thumbnail and uploads it to the blob storage container <see
    /// cref="BlobStorage.Containers.ImgConversions"/>. <para> If an exception occurs during processing, the message is
    /// marked as completed to prevent retries. Ensure that the input metadata contains a valid image file URL, as
    /// unsupported file types may cause the operation to fail. </para></remarks>
    /// <param name="incoming">The received Service Bus message containing the event data.</param>
    /// <param name="messageActions">Provides actions for completing, abandoning, or deferring the Service Bus message.</param>
    /// <param name="input">The metadata payload containing information about the source image and its properties.</param>
    /// <returns>A <see cref="ServiceBusMessageDTO"/> containing the event details and metadata after the thumbnail creation
    /// process is completed.</returns>
    [Function(nameof(CreateThumbnail))]
    public async Task Run(
        [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.CreateThumbnail, Connection = Secrets.FUMSGI)]
        ServiceBusReceivedMessage incoming,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("{correlationId} Request to create thumbnail. Inspecting message {messageId}...", incoming.CorrelationId, incoming.MessageId);
        if (!await ValidateReceivedMessage(incoming, messageActions)) return; // DLQ + skip if not valid 
        const string destination = BlobStorage.Containers.Thumbnails;
        var size = (ThumbnailSize)incoming.ApplicationProperties[ServiceBus.Props.ThumbnailSize];
        BlobMetaDTO metadata = GetPreconfiguredDTO(incoming);
        _logger.LogInformation("{correlationId} Validated thumbnail request for blob {fileName}", incoming.CorrelationId, metadata.Name);
        try
        {
            ///TODO: Implement **better validation** on file type before CloudCanvas v1.0, 
            /// for example, what if this function receives a .pdf file? or a .mp4, .zip etc...
            var bclient = await _blobService.GetOrCreateContainerClientAsync(metadata.ContainerName); // original file blob container
            var stream = await bclient.GetBlobClient(metadata.Name).OpenReadAsync(); // download file
            using var thumbnail = await ImageTool.ResizeAsync(stream, size); // Create thumbnail
            var props = BlobStorageService.SetOriginalMetadata(metadata.OriginalFilename, metadata.UploadedBy!); // Set thumbnail specific metadata
            props.Add("size", size.ToString());

            BlobMetaDTO thumbnailMeta = await _blobService.UploadAsync(thumbnail, metadata.OriginalFilename, props, destination, $"{metadata.Id}_{size.ToString()}"); 
            metadata.Thumbnails.Add(size, thumbnailMeta.Url);
            _logger.LogInformation("{correlationId} {size} Thumbnail created and saved to {thumbnails}/{blobName}.", incoming.CorrelationId, size.ToString(), destination, metadata.Name);
        } catch (Exception e)
        {
            await messageActions.AbandonMessageAsync(incoming);  // Drop the ball & Run away
            _logger.LogError(e, "{correlationId} Failed to Create {size} Thumbnail for blob {originalFilename}.\n Message ({messageId}) abandoned.", incoming.CorrelationId, size.ToString(), metadata.OriginalFilename, incoming.MessageId);
            return;

        }

        var response = MessageFactory.BuildFor(metadata) // Manual dispatch required for full control (dotnet-isolated)
            .WithSubject($"{ServiceBus.Status.ThumbnailCreated}:{destination}/{metadata.OriginalFilename}.") // Add Subject
            .AddProperty(ServiceBus.Props.EventType, ServiceBus.Subs.CreateThumbnail) // So that it makes it through subscription filters
            .AddProperty(ServiceBus.Props.ThumbnailSize, size.ToString()) // Let's make it through even more subscription filters
            .Finalize(incoming.CorrelationId); // Finalize and return the message
        await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, response); // Send the message and call it a day
        _logger.LogInformation("{correlationId} Sent Message '{messageId}' to topic '{topic}': {subject}", response.CorrelationId, response.MessageId, ServiceBus.Topics.FileUpdates, response.Subject);
    }

    private BlobMetaDTO GetPreconfiguredDTO(ServiceBusReceivedMessage incoming)
    {
        var metadata = CCSerializer.MetaFromBinaryData<BlobMetaDTO>(incoming.Body);
        // Add thumbnail related metadata
        metadata.ProcessingStage = (int)BlobProcessingStage.CreateThumbnail;
        metadata.LastModified = DateTime.UtcNow;
        return metadata;
    }

    private async Task<bool> ValidateReceivedMessage(ServiceBusReceivedMessage incoming, ServiceBusMessageActions messageActions)
    {
        try { Validate.SBMessageSize(incoming, _maxMessageLength); }
        catch (MessageTooLargeException e) // Unexpectedly larg? considered unsafe, discard
        {
            _logger.LogWarning(e, "{correlationId} Message '{messageId}' too large. Size: {messageLength}/{maxMessageLength} Bytes. DLQ Material -> Skipping...", incoming.CorrelationId, incoming.MessageId, e.ActualMessageSize, e.MaxMessageSize);
            await messageActions.DeadLetterMessageAsync(incoming, deadLetterReason: nameof(MessageTooLargeException), deadLetterErrorDescription: e.Message); // Skip and throw awway
            return false;
        }
        return true;
    }
}
