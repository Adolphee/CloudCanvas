using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrchestrationStarter
{
    public class OrchestrationStarter
    {
        [FunctionName("OrchestrationStarter")]
        public async Task Run([ServiceBusTrigger(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.ExtractMetaData, Connection = BlobStorage.Self)]
        ServiceBusReceivedMessage message,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            var dto = JsonSerializer.Deserialize<BlobMetaDTO>(message.Body);
            var instanceId = await starter.StartNewAsync("FileProcessingOrchestrator", dto);
            log.LogInformation($"Orchestration started with ID: {instanceId}");
        }
    }
}
