using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs.Models;
using CloudCanvas.Constants;
using CloudCanvas.Functions.Constants;
using CloudCanvas.Functions.DTOs;
using CloudCanvas.Functions.Services;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    private readonly ServiceBusAdapter _adapter;
    private readonly BlobMetaConverter _converter;

    public ExtractMetadata(ILogger<ExtractMetadata> logger, ServiceBusAdapter serviceBusAdapter, BlobMetaConverter metaConverter)
    {
        _logger = logger;
        _adapter = serviceBusAdapter;
        _converter = metaConverter;
    }

    [Function(nameof(ExtractMetadata))]
    public async Task Run(
        [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.ExtractMetaData, Connection = ServiceBus.Topics.FileUpdate.Listen)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var body = System.Text.Encoding.UTF8.GetString(message.Body);

        string pathToMainSchema = Path.Combine(AppContext.BaseDirectory, "Schemas", "servicebus-message.schema.json");
        string pathToBlobMetaSchema = Path.Combine(AppContext.BaseDirectory, "Schemas", "blob-metadata.schema.json");

        string MainSchemaJson = File.ReadAllText(pathToMainSchema);
        string BlobMetaSchemaJson = File.ReadAllText(pathToBlobMetaSchema);

        var resolver = new JSchemaPreloadedResolver();
        resolver.Add(new Uri("blob-metadata.schema.json", UriKind.RelativeOrAbsolute), System.Text.Encoding.UTF8.GetBytes(BlobMetaSchemaJson));

        var schema = JSchema.Parse(MainSchemaJson, resolver);
        JObject obj;
        try
        {
            obj = JObject.Parse(body);
            IList<string> errors;
            bool isValid = obj.IsValid(schema, out errors);
            if (isValid)
            {
                var payload = System.Text.Json.JsonSerializer.Deserialize<CloudCanvasMessageDTO>(body);

                _logger.LogWarning("Message.ID: {id}", message.MessageId);
                _logger.LogWarning("Message.Body: {body}", _converter.ToString(payload));

                if (payload.Event.Contains("Start"))
                {
                    _logger.LogInformation("EXTRACT METADATA PROCESSING HERE .... !!!!!!!!!!!!!!");
                    await _adapter.SendAsync(ServiceBus.Topics.FileUpdates, new ServiceBusMessage("EXTRACT METADATA PROCESSING HERE .... !!!!!!!!!!!!!!"));
                }
                else
                {
                    _logger.LogWarning("SKIPPING METADATA EXTRACTION... SHOULD BE ALREADY DONE.");
                }
            }
            else
            {
                _logger.LogError("INVALID CloudCanvas Message... Does not comply with the standards set in the 'main' schema.");
            }
        }
        catch (JsonReaderException e)
        {
            _logger.LogError("Alien message format: considered hostile and ignored. Exception Message:", e.Message);
        }
        await Task.CompletedTask;
    }
}