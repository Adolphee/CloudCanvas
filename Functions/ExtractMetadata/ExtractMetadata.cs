using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs.Models;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;


namespace CloudCanvas.Functions;

public class ExtractMetadata(ILogger<ExtractMetadata> logger, BlobStorageService blobSerivce, ServiceBusAdapter service, BlobMetadataSerializer serializer)
{
    private readonly ILogger<ExtractMetadata> _logger = logger;
    private readonly BlobStorageService _blobSerivce = blobSerivce;
    private readonly ServiceBusAdapter _sbAdapter = service;
    private readonly BlobMetadataSerializer _serializer = serializer;

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
    public async Task Run([BlobTrigger(BlobStorage.Containers.Uploads + "/{name}", Connection = BlobStorage.Self)] Stream input, string name)
    {
        _logger.LogInformation($"{ServiceBus.Topics.FileUpdates}, {ServiceBus.Subs.ExtractMetaData}, {name}");

        const string uploads = BlobStorage.Containers.Uploads;
        var cclient = await _blobSerivce.GetContainerClientAsync(uploads);
        var blob = cclient.GetBlobClient(name);
        BlobProperties props = blob.GetProperties();
        BlobMetaDTO metadata = _serializer.FromBlobProperties(name, blob.Uri.ToString(), blob.GetProperties());

        var message = new ServiceBusMessage(_serializer.Serialize(metadata));
        message.Subject = "Metadata Extracted - file ready for processing.";
        // Since I am manually handling messages, I am also responsible for serialization etc.
        // These properties will help the _serializer in another azfunction to deserialize
        message.ApplicationProperties.Add("blobUrl", metadata.BlobUrl);
        message.ApplicationProperties.Add("originalFileName", metadata.OriginalFileName);

        // Tweaking so the message goes through subscription filters on function CreateThumbnail
        message.ApplicationProperties.Add(ServiceBus.Props.EventType, ServiceBus.Subs.ExtractMetaData);
        // I am forced to create to call for a client and send the message manually,
        // because for dotnet-isolated functions there is no IAsyncCollector<ServiceBusMessage> I can call
        // to set ApplicationProperties on the message, which I MUST to do for subscription filtering (example: persist-metadata) to work
        await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, message);
        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}", name, _serializer.Serialize(metadata));
    }
}