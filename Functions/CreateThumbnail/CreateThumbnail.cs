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
        ServiceBusMessageActions messageActions, ThumbnailSize size = ThumbnailSize.Small)
    {
        _logger.LogInformation("Function {functionName} started & wired up successfully. Looking for a job...", nameof(CreateThumbnail));
        try { Validate.SBMessageSize(incoming, _maxMessageLength); }
        catch (MessageTooLargeException e) // Unexpectedly larg? considered unsafe, discard
        {
            _logger.LogWarning(e, "[{correlationId}] Message '{messageId}' too large. Size: {messageLength}/{maxMessageLength} Bytes. DLQ Material -> Skipping...", incoming.CorrelationId, incoming.MessageId, e.ActualMessageSize, e.MaxMessageSize);
            await messageActions.DeadLetterMessageAsync(incoming); // Skip and throw awway
            throw;
        }
        const string thumbnails = BlobStorage.Containers.Thumbnails;
        const string uploads = BlobStorage.Containers.Uploads;
        int intSize = (int) incoming.ApplicationProperties[ServiceBus.Props.ThumbnailSize];
        var metadata = CCSerializer.FromBinaryData<BlobMetaDTO>(incoming.Body);
        ThumbnailSize altSize = ImageTool.GetThumbnailSize(intSize);
        _logger.LogInformation("[{correlationId}][START] Creating thumbnail for {fileName} at destination: {thumbnails}/{originalFileName}.", incoming.CorrelationId, metadata.OriginalFileName, thumbnails, metadata.OriginalFileName);
        try
        {
            ///TODO: Implement **better validation** on file type before CloudCanvas v1.0, 
            /// for example, what if this function receives a .pdf file? or a .mp4, .zip etc...
            var bclient = await _blobService.GetOrCreateContainerClientAsync(uploads); // original file blob container
            var stream = await bclient.GetBlobClient(metadata.OriginalFileName).OpenReadAsync(); // download file
            bclient = await _blobService.GetOrCreateContainerClientAsync(thumbnails); // switch to thumbnails desination container
            using var output = await ImageTool.ResizeAsync(stream, size); // Create thumbnail
            await _blobService.UploadAsync(output, metadata.OriginalFileName!, thumbnails); //upload the thumbnail to destionation
        } catch (Exception e)
        {
            _logger.LogCritical(e, "[{correlationId}][CRIT] Unable to Create {size} Thumbnail for blob {originalFilename}.\n Message ({messageId}) abandoned.", incoming.CorrelationId, size.ToString(), metadata.OriginalFileName, incoming.MessageId);
            await messageActions.AbandonMessageAsync(incoming);  // Drop the ball & Run away
            throw;
        }

        _logger.LogInformation("[{correlationId}][OK] Created thumbnail for blob '{blobName}' and saved to {thumbnails}/{originalFileName}.", incoming.CorrelationId, metadata.OriginalFileName, thumbnails, metadata.OriginalFileName);
        var response = MessageFactory.BuildFor(metadata) // Manual dispatch required for full control (dotnet-isolated)
            .WithSubject($"{ServiceBus.Status.ThumbnailCreated}:{thumbnails}/{metadata.OriginalFileName}.") // Add Subject
            .AddProperty(ServiceBus.Props.EventType, ServiceBus.Subs.CreateThumbnail) // So that it makes it through subscription filters
            .AddProperty(ServiceBus.Props.ThumbnailSize, size.ToString()) // Let's make it through even more subscription filters
            .Finalize(incoming.CorrelationId); // Finalize and return the message
        await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, response); // Send the message and call it a day
        _logger.LogInformation("[{correlationId}][DONE] Sent Message '{messageId}' to topic '{topic}': {subject}", response.CorrelationId, response.MessageId, ServiceBus.Topics.FileUpdates, response.Subject);
    }
}
