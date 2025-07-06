using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using CloudCanvas.Shared.Constants;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace FileProcessingOrchestrator
{
    public class FileProcessingOrchestrator
    {
        [FunctionName("FileProcessingOrchestrator")]
        public async Task Run([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
        {
            log.LogInformation($"Orchestrator started with instance ID: {context.InstanceId}");
            //context.CallActivityAsync("ExtractMetadataActivity", new RetryOptions(TimeSpan.FromSeconds(5), 3), context.GetInput<string>());
            await context.CallActivityAsync("ProcessImageActivity", context.GetInput<string>());
            log.LogInformation($"Orchestrator finished with instance ID: {context.InstanceId}");
        }
    }
}
