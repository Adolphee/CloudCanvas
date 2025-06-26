using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs.Models;
using CloudCanvas.Functions.DTOs;
using CloudCanvas.Functions.Services;
using CloudCanvas.Services;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;


namespace CloudCanvas_Functions;

public class ExtractMetadata
{
    private readonly ILogger<ExtractMetadata> _logger;
    private readonly BlobStorageService _blobSerivce;
    private readonly ServiceBusAdapter _sbAdapter;
    private readonly BlobMetaConverter _converter;

    public ExtractMetadata(ILogger<ExtractMetadata> logger, BlobStorageService blobSerivce, ServiceBusAdapter service, BlobMetaConverter converter)
    {
        _logger = logger;
        _blobSerivce = blobSerivce;
        _sbAdapter = service;
        _converter = converter;
    }

    /// <summary>
    /// Processes a blob triggered by an upload event, extracts metadata, and sends a message to a Service Bus topic.
    /// </summary>
    /// <remarks>This method is triggered by a blob upload to the specified container. It extracts metadata
    /// from the blob, logs the processing details, and sends a message to the Service Bus topic for further
    /// processing.</remarks>
    /// <param name="input">The stream representing the uploaded blob content.</param>
    /// <param name="name">The name of the uploaded blob.</param>
    /// <returns>A <see cref="CloudCanvasMessageDTO"/> containing the event details, subject, and extracted metadata.</returns>
    [Function(nameof(ExtractMetadata))]
    [ServiceBusOutput(ServiceBus.Topics.FileUpdates, Connection = ServiceBus.Topics.FileUpdate.Send)]
    public async Task Run([BlobTrigger(BlobStorage.Containers.Uploads + "/{name}", Connection = BlobStorage.Self)] Stream input, string name)
    {
        _logger.LogInformation(ServiceBus.GetRealEventString(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.ExtractMetaData, name));

        const string uploads = BlobStorage.Containers.Uploads;
        var cclient = await _blobSerivce.GetContainerClientAsync(uploads);
        var blob = cclient.GetBlobClient(name);
        BlobProperties props = blob.GetProperties();
        BlobMetaDTO metadata = _converter.ToBlobMeta(name, blob.Uri.ToString(), props);

        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}", name, metadata);

        var message = new ServiceBusMessage(_converter.ToString(metadata));
        message.Subject = "Metadata Extracted - file ready for processing.";
        // Since I am manually handling messages, I am also responsible for serialization etc.
        // These properties will help the _converter in another azfunction to deserialize
        message.ApplicationProperties.Add("blobUrl", metadata.BlobUrl);
        message.ApplicationProperties.Add("originalFileName", metadata.OriginalFileName);


        // Tweaking so the message goes through subscription filters on function CreateThumbnail
        message.ApplicationProperties.Add(ServiceBus.Props.EventType, ServiceBus.Subs.ExtractMetaData);
        // I am forced to create to call for a client and send the message manually,
        // because for dotnet-isolated functions there is no IAsyncCollector<ServiceBusMessage> I can call
        // to set ApplicationProperties on the message, which I MUST to do for subscription filtering (example: persist-metadata) to work
        await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, message);
        await Task.CompletedTask;
    }
}