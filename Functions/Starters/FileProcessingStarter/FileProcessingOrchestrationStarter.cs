using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CloudCanvas.Shared.Constants;
using Azure.Messaging.ServiceBus;

namespace CloudCanvas.Functions.Starters
{
    public static class FileProcessingOrchestrationStarter
    {
        [FunctionName(nameof(FileProcessingOrchestrationStarter))]
        public static async Task<IActionResult> Run(
            [ServiceBusTrigger(ServiceBus.Topics.FileUpdates, Connection = BlobStorage.Self)] ServiceBusMessage message,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            if (string.IsNullOrWhiteSpace(message.Body.ToString()))
            {
                return new BadRequestObjectResult("Missing required 'blobName' parameter.");
            }

            string instanceId = await starter.StartNewAsync("FileProcessingOrchestrator", message.Body);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return instanceId != null
                ? (ActionResult)new OkObjectResult($"Started orchestration with ID = '{instanceId}'.")
                : new BadRequestObjectResult("Failed to start orchestration.");
        }
    }
}