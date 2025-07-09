using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using CloudCanvas.Shared.Constants;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;
using CloudCanvas.Shared.DTOs;

namespace FileProcessingOrchestrator
{
    public class FileProcessingOrchestrator
    {
        [FunctionName("FileProcessingOrchestrator")]
        public async Task Run([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var blobmeta = context.GetInput<BlobMetaDTO>();
            log.LogInformation($"Orchestrator started with instance ID: {context.InstanceId}");
            await context.CallActivityAsync("ExtractMetadataActivity", blobmeta);
            await context.CallActivityAsync("CreateThumbnailActivity", blobmeta);
            log.LogInformation($"Orchestrator finished with instance ID: {context.InstanceId}");
        }
    }
}
