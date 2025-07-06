using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CloudCanvas.Shared.Constants;

namespace CloudCanvas.Functions.Starters
{
    public static class DurableFunctionsEntityHttpCSharp
    {
        [FunctionName("DurableFunctionsEntityCSharp_CounterHttpStart")]
        public static async Task<IActionResult> Run(
            [BlobTrigger("uploads/{name}", Connection = BlobStorage.Self)] string name,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return new BadRequestObjectResult("Missing required 'blobName' parameter.");
            }

            string instanceId = await starter.StartNewAsync("FileProcessingOrchestrator", name);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return instanceId != null
                ? (ActionResult)new OkObjectResult($"Started orchestration with ID = '{instanceId}'.")
                : new BadRequestObjectResult("Failed to start orchestration.");
        }
        
        [FunctionName(nameof(Counter))]
        public static void Counter([EntityTrigger] IDurableEntityContext context)
        {
            switch (context.OperationName.ToLowerInvariant())
            {
                case "add":
                    context.SetState(context.GetState<int>() + context.GetInput<int>());
                    break;
                case "reset":
                    context.SetState(0);
                    break;
                case "get":
                    context.Return(context.GetState<int>());
                    break;
            }
        }
    }
}