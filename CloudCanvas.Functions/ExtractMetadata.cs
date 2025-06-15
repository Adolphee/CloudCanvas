using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs.Models;
using CloudCanvas.Constants;
using CloudCanvas.Functions.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Schema;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using static CloudCanvas.Constants.BlobStorage;

namespace CloudCanvas.Functions;

/// <summary>
/// Extracts Metadata from uploaded blobs, received as DTO objects through Service Bus Messages
/// </summary>
public class ExtractMetadata
{
    private readonly ILogger<ExtractMetadata> _logger;
    private readonly IConfiguration _config;
    private readonly ServiceBusAdapter _adapter;

    public ExtractMetadata(ILogger<ExtractMetadata> logger, IConfiguration config, ServiceBusAdapter serviceBusAdapter)
    {
        _logger = logger;
        _config = config;
        _adapter = serviceBusAdapter;
    }

    [Function(nameof(ExtractMetadata))]
    public async Task Run(
        [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.ExtractMetaData, Connection = ServiceBus.Topics.FileUpdate.Listen)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        // TODO: change this back to logInformation or remove it completely
        _logger.LogWarning("Message.ID: {id}", message.MessageId);
        _logger.LogWarning("Message.Body: {body}", message.Body);
        _logger.LogWarning("Message.Body: {body}", message.Body);
        //_logger.LogWarning("Message.ApplicationProperties: {props}", message.ApplicationProperties);
        //_logger.LogWarning("Message Content-Type: {contentType}", message.ContentType);


        var pyload = JsonSerializer.Deserialize<BlobMetaDTO>(message.Body);
        string schemaFile = File.ReadAllText($"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}/schemas/blob-metadata.schema.json");
        var schema = JSchema.Parse(schemaFile);


        if (message.ApplicationProperties.ContainsKey(BlobMeta.Properties))
        {
            //BlobProperties props = (BlobProperties)message.ApplicationProperties.Single(x => x.Key == "Properties").Value;
            var name = message.ApplicationProperties.Single(x => x.Key == BlobMeta.OriginalFileName).Value;
            var url = message.ApplicationProperties.Single(x => x.Key == BlobMeta.BlobUrl).Value;
            var msg = new ServiceBusMessage($"Blob-Trigger:{ServiceBus.Topics.FileUpdates}:{ServiceBus.Subs.ExtractMetaData}:Done [{name}].");
            msg.ApplicationProperties.Add(BlobMeta.ProcessingStage, $"{ServiceBus.Subs.ExtractMetaData}:Done");
            msg.ApplicationProperties.Add(BlobMeta.OriginalFileName, name);
            msg.ApplicationProperties.Add(BlobMeta.BlobUrl, url);
            msg.ApplicationProperties.Add(BlobMeta.Project, ServiceBus.Subs.ExtractMetaData);
            
            //msg.ApplicationProperties.Add(BlobStorage.BlobMeta.CreatedOn, props.CreatedOn);
            //msg.ApplicationProperties.Add(BlobStorage.BlobMeta.OriginalImageFormat, props.ContentType);
            msg.ApplicationProperties.Add(BlobStorage.BlobMeta.UploadedBy, "anonymous"); // for now it's anonymous by default, but I will later implement authentication
            await _adapter.SendAsync(ServiceBus.Topics.FileUpdates, msg);
        }
       

        // TODO: azfunc -> pickup msg from topic -> do something -> send message to servicebus
        await Task.CompletedTask;
    }
}