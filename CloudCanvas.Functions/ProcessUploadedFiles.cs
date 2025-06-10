using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using CloudCanvas.Constants;
using SixLabors.ImageSharp.Processing;
using System.Configuration;
using CloudCanvas.Services;
using System.IO.Pipelines;
namespace CloudCanvas_Functions;

public class ProcessUploadedFiles
{
    private readonly ILogger<ProcessUploadedFiles> _logger;
    private readonly IConfiguration _config;
    private readonly BlobStorageService _blobSerivce;

    public ProcessUploadedFiles(ILogger<ProcessUploadedFiles> logger, IConfiguration config, BlobStorageService blobSerivce)
    {
        _logger = logger;
        _config = config;
        _blobSerivce = blobSerivce;
    }

    [Function(nameof(ProcessUploadedFiles))]
    public async Task Run([BlobTrigger($"{AzureBlobStorage.Containers.Uploads}" + "/{name}", Connection = "AzureWebJobsStorage")] Stream input, string name)
    {
        _logger.LogWarning($"Preparing to process file: {name}");

        const string uploads = AzureBlobStorage.Containers.Uploads;
        const string conversions = AzureBlobStorage.Containers.ImgConversions;
        if (input.CanSeek)
        {
            _logger.LogWarning($"Blob input for file '{name}' is seekable. Attempting to process it...");
            try
            {
                using var image = await Image.LoadAsync(input);
                image.Mutate(i => i.Resize(200, 200));
                using var output = new MemoryStream();
                await image.SaveAsJpegAsync(output);
                output.Position = 0;
                await _blobSerivce.UploadAsync(conversions, output, name);
                _logger.LogInformation($"Successfully processed and saved blob [{name}].");
            }
            catch (Exception e)
            {
                _logger.LogDebug(e.Message, e.StackTrace);
                throw e;
            }
        }
        else
        {
            _logger.LogWarning($"Can't find Blob['{name}'] in Storage container '{uploads}'.");
        }
        _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}", name, input);
        return;
    }
}