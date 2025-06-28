using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using CloudCanvas.Shared;

namespace CloudCanvas_Functions;
public class CreateThumbnail
{
    private readonly ILogger<CreateThumbnail> _logger;
    private readonly BlobStorageService _blobService;
    private readonly BlobMetaConverter _converter;
    private readonly ServiceBusAdapter _sbAdapter;

    public CreateThumbnail(ILogger<CreateThumbnail> logger, BlobStorageService blobService, BlobMetaConverter converter, ServiceBusAdapter adapter)
    {
        _logger = logger;
        _blobService = blobService;
        _converter = converter;
        _sbAdapter = adapter;
    }

    /// <summary>
    /// Processes a Service Bus message to create a thumbnail image from the provided metadata and uploads it to a
    /// specified blob storage container.
    /// </summary>
    /// <remarks>This function listens to the Service Bus topic <see cref="ServiceBus.Topics.FileUpdates"/>
    /// and processes messages from the subscription <see cref="ServiceBus.Subs.CreateThumbnail"/>. It resizes the image
    /// specified in the metadata to a 50x50 thumbnail and uploads it to the blob storage container <see
    /// cref="BlobStorage.Containers.ImgConversions"/>. <para> If an exception occurs during processing, the message is
    /// marked as completed to prevent retries. Ensure that the input metadata contains a valid image file URL, as
    /// unsupported file types may cause the operation to fail. </para></remarks>
    /// <param name="message">The received Service Bus message containing the event data.</param>
    /// <param name="messageActions">Provides actions for completing, abandoning, or deferring the Service Bus message.</param>
    /// <param name="input">The metadata payload containing information about the source image and its properties.</param>
    /// <returns>A <see cref="ServiceBusMessageDTO"/> containing the event details and metadata after the thumbnail creation
    /// process is completed.</returns>
    [Function(nameof(CreateThumbnail))]
    public async Task Run(
        [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.CreateThumbnail, Connection = ServiceBus.Topics.FileUpdate.Listen)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        const string thumbnails = BlobStorage.Containers.Thumbnails;
        const string uploads = BlobStorage.Containers.Uploads;
        BlobMetaDTO metadata = _converter.FromBinaryData(message.Body); //TODO: wrap in try-catch block
        metadata.BlobUrl = message.ApplicationProperties["blobUrl"].ToString() ?? ""; // TODO: also wrap in try-catch or validate some other way
        metadata.OriginalFileName = message.ApplicationProperties["originalFileName"].ToString() ?? ""; //TODO: idem
        try
        {
            ///TODO: Implement **better validation** on file type, 
            /// for example, what if this function receives a .pdf file? or a .mp4, .zip etc...
            var bclient = await _blobService.GetContainerClientAsync(uploads); // original file blob container
            var stream = await bclient.GetBlobClient(metadata.OriginalFileName).OpenReadAsync(); // download file
            bclient = await _blobService.GetContainerClientAsync(thumbnails); // switch to thumbnails desination container
            using var image = await Image.LoadAsync(stream);
            image.Mutate(i => i.Resize(50, 50)); // In the real world, more complex operations can be performed in this step
            using var output = new MemoryStream();
            await image.SaveAsJpegAsync(output);
            output.Position = 0;
            await _blobService.UploadAsync(thumbnails, output, metadata.OriginalFileName); //upload the thumbnail
            _logger.LogInformation($"Successfully created thumbnail and saved to [{thumbnails}]/{metadata.OriginalFileName}.");
        }
        catch (Exception e) { 
            _logger.LogError($"Failed Create Thumbnail. Service Bus Message [{ServiceBus.Subs.CreateThumbnail}]:\n{message.ToString()}");
            _logger.LogDebug(e.Message, e.StackTrace);
            await messageActions.AbandonMessageAsync(message);  // Complete the message
        }
        var body = new ExtractMetadataMessageDTO
        {
            Event = ServiceBus.GetRealEventString(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.CreateThumbnail, "done"),
            Subject = "Thumbnail Created.",
            Payload = metadata
        };

        var responseMessage = new ServiceBusMessage(_converter.Serialize(body));
        // Tweaking so the message goes through subscription filters on function CreateThumbnail
        responseMessage.ApplicationProperties.Add(ServiceBus.Props.EventType, ServiceBus.Subs.CreateThumbnail);
        // I am forced to create to call for a client and send the message manually,
        // because for dotnet-isolated functions there is no IAsyncCollector<ServiceBusMessage> I can call
        // to set ApplicationProperties on the message, which I MUST to do for subscription filtering (example: persist-metadata) to work
        await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, responseMessage);
        await Task.CompletedTask;
    }
}