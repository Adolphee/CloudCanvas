using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace CloudCanvas.Functions;

/// <summary>
/// Extracts Metadata from uploaded blobs, received as DTO objects through Service Bus Messages
/// </summary>
/// TODO: Pull this value from the configuration file
public class PersistMetadata(ILogger<PersistMetadata> logger, ServiceBusAdapter serviceBusAdapter, BlobMetadataSerializer metaConverter, CosmosClientWrapper cosmos)
{
    private readonly ILogger<PersistMetadata> _logger = logger;
    private readonly ServiceBusAdapter _sbAdapter = serviceBusAdapter;
    private readonly BlobMetadataSerializer _serializer = metaConverter;
    private readonly CosmosClientWrapper _cosmos = cosmos;
    private const int MaxMessageLength = 16 * 1024;

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
        [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.PersistMetadata, Connection = ServiceBus.Topics.FileUpdate.Listen)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        BlobMetaDTO metadata = _serializer.FromBinaryData(message.Body); //TODO: wrap in try-catch block, conversion may fail
        metadata.BlobUrl = message.ApplicationProperties["blobUrl"].ToString() ?? ""; // TODO: also wrap in try-catch or validate some other way
        metadata.OriginalFileName = message.ApplicationProperties["originalFileName"].ToString() ?? ""; //TODO: idem
        metadata = await _cosmos.SaveAsync(metadata, CloudCosmos.Containers.BlobMeta); // Push metadata to CosmosDB
        var responseMessage = new ServiceBusMessage(_serializer.Serialize(metadata));
        responseMessage.Subject = "Metadata Extracted - file ready for processing.";
        // Tweaking so the message goes through subscription filters on function CreateThumbnail
        responseMessage.ApplicationProperties.Add(ServiceBus.Props.EventType, ServiceBus.Subs.PersistMetadata);
        // I am forced to create to call for a client and send the message manually,
        // because for dotnet-isolated functions there is no IAsyncCollector<ServiceBusMessage> I can call
        // to set ApplicationProperties on the message, which I MUST to do for subscription filtering (example: persist-metadata) to work
        await _sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, responseMessage);
    }

    #region Validation
    /// <summary>
    /// Analyzes a received Service Bus message to validate its format and schema compliance.
    /// </summary>
    /// <remarks>This method performs the following checks: <list type="bullet"> <item><description>Ensures
    /// the message body does not exceed the maximum allowed size.</description></item> <item><description>Validates the
    /// message body against a predefined JSON schema.</description></item> <item><description>Handles unrecognized or
    /// malformed JSON formats gracefully by logging the issue and returning an error message.</description></item>
    /// </list> If the message is invalid or unrecognized, the method logs the issue and returns a descriptive error
    /// message. If the message is valid, an empty string is returned.</remarks>
    /// <param name="message">The <see cref="ServiceBusReceivedMessage"/> to scrutinize. The message body is expected to be a JSON string.</param>
    /// <returns>A string containing an error message if the message is invalid, too large, or unrecognized; otherwise, an empty
    /// string if the message is valid.</returns>
    public string ScrutinizeSBMessage(ServiceBusReceivedMessage message)
    {
        if (message.Body.ToMemory().Length > MaxMessageLength) // if the message is too large, I'm not taking any risks
        {
            _logger.LogWarning("{topic}({subscription}): MESSAGE TOO LARGE. SKIPPING message {messageId}...", 
               ServiceBus.Topics.FileUpdates, ServiceBus.Subs.PersistMetadata, message.MessageId);
            return String.Empty;
        }

        IList<string> errors;
        var body = System.Text.Encoding.UTF8.GetString(message.Body);
        var schema = GetCloudCanvasMainJsonSchema();
        JObject obj;
        try
        {
            obj = JObject.Parse(body);
            var isValid = obj != null ? obj.IsValid(schema, out errors) : false;
            if (!isValid)
            {
                _logger.LogError("INVALID CloudCanvas Message '{messageId}'... Does not comply with the standards set in the 'main' JsonSchema.", message.MessageId);
            }
        }
        catch (JsonReaderException e)
        {
            _logger.LogError(e, "UNRECOGNISED message format: considered hostile and ignored. Exception Message: {message}", e.Message);
            throw;
        }

        return String.Empty;
    }

    /// <summary>
    /// Loads and returns the main JSON schema for CloudCanvas, resolving any referenced subschemas.
    /// </summary>
    /// <remarks>This method reads the primary schema file and a referenced subschema file from the
    /// application's base directory. It resolves the subschema using a preloaded resolver to ensure all schema
    /// dependencies are properly handled.</remarks>
    /// <returns>A <see cref="JSchema"/> object representing the main JSON schema for CloudCanvas, with all references resolved.</returns>
    public JSchema GetCloudCanvasMainJsonSchema()
    {
        string pathToMainSchema = Path.Combine(AppContext.BaseDirectory, "Schemas", "servicebus-message.schema.json");
        string pathToBlobMetaSchema = Path.Combine(AppContext.BaseDirectory, "Schemas", "blob-metadata.schema.json");

        string MainSchemaJson = File.ReadAllText(pathToMainSchema);
        string BlobMetaSchemaJson = File.ReadAllText(pathToBlobMetaSchema);

        var resolver = new JSchemaPreloadedResolver();
        resolver.Add(new Uri("blob-metadata.schema.json", UriKind.RelativeOrAbsolute), System.Text.Encoding.UTF8.GetBytes(BlobMetaSchemaJson));

        var schema = JSchema.Parse(MainSchemaJson, resolver);
        return schema;
    }
    #endregion
}