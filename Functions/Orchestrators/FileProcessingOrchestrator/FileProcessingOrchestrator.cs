using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;
using CloudCanvas.Shared.DTOs;

namespace CloudCanvas.Functions.Durable.Orchestrators
{
    public class FileProcessingOrchestrator
    {
        [FunctionName("FileProcessingOrchestrator")]
        public static async Task Run([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            var blobmeta = context.GetInput<BlobMetaDTO>();
            log.LogInformation($"Orchestrator started with instance ID: {context.InstanceId}");
            await context.CallActivityAsync("ExtractMetadataActivity", blobmeta);
            await context.CallActivityAsync("CreateThumbnailActivity", blobmeta);
            log.LogInformation($"Orchestrator finished with instance ID: {context.InstanceId}");
        }
    }
}
