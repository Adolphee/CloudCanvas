using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs.Models;
using CloudCanvas.Constants;
using CloudCanvas.Functions.Constants;
using CloudCanvas.Services;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Interfaces;
using CloudCanvas.Shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Configuration;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace CloudCanvas_Functions;

public class ProcessUploadedFiles
{
    private readonly ILogger<ProcessUploadedFiles> _logger;
    private readonly IConfiguration _config;
    private readonly BlobStorageService _blobSerivce;
    private readonly ServiceBusAdapter _service;

    public ProcessUploadedFiles(ILogger<ProcessUploadedFiles> logger, IConfiguration config, BlobStorageService blobSerivce, ServiceBusAdapter service)
    {
        _logger = logger;
        _config = config;
        _blobSerivce = blobSerivce;
        _service = service;
    }

    [Function(nameof(ProcessUploadedFiles))]
    public async Task Run([BlobTrigger(BlobStorage.Containers.Uploads + "/{name}", Connection = BlobStorage.Self)] Stream input, string name)
    {
        const string uploads = BlobStorage.Containers.Uploads;
        const string conversions = BlobStorage.Containers.ImgConversions;
        _logger.LogInformation($"[Blob-Trigger][activated]: {name}.");
        // send notification to service bus so that ExtractMetadata can wake up as we proceed
        var message = new ServiceBusMessage($"Blob-Trigger:{ServiceBus.Topics.FileUpdates}:{ServiceBus.Subs.ExtractMetaData}:Start [{name}].");
        var cclient = await _blobSerivce.GetContainerClientAsync(BlobStorage.Containers.Uploads);
        var blob = cclient.GetBlobClient(name);
        var converter = new BlobMetaConverter();
        BlobProperties props = blob.GetProperties();
        BlobMetaDTO meta = converter.ToBlobMeta(blob.Uri.ToString(), props);
        // add the url to the message so we can find 
        message.ApplicationProperties.Add("BlubUrl", blob.Uri);
        message.ApplicationProperties.Add(BlobStorage.BlobMeta.OriginalFileName, name);
        message.ApplicationProperties.Add("ContentType", blob.GetProperties().Value.ContentType);
        message.ApplicationProperties.Add("Properties", blob.GetProperties().Value.ToString());
        await _service.SendAsync(ServiceBus.Topics.FileUpdates, message);
        if (input.CanSeek)
        {
            _logger.LogInformation($"Blob stream for file '{name}' is seekable. Preparing to process it...");
            try
            {
                using var image = await Image.LoadAsync(input);
                image.Mutate(i => i.Resize(200, 200)); //Just to illustrate "image processing"--in reality, more complex operations can be performed in this step
                using var output = new MemoryStream();
                await image.SaveAsJpegAsync(output);
                output.Position = 0;
                await _blobSerivce.UploadAsync(conversions, output, name);
                _logger.LogInformation($"Successfully processed and saved blob [{name}].");
            }
            catch (Exception e) { _logger.LogDebug(e.Message, e.StackTrace); }
        }
        else
        {
            _logger.LogWarning($"Can't find Blob['{name}'] in Storage container '{uploads}'.");
        }
        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}", name, input);
        return;
    }

}