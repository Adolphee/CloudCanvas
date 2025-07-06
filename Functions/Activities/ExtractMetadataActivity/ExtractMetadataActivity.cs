using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ExtractMetadataActivity
{
    public class ExtractMetadataActivity
    {
        [FunctionName("ExtractMetadataActivity")]
        public void Run([ActivityTrigger] string blobUrl, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{blobUrl} \n Size: {blobUrl.Length} Bytes");
        }
    }
}
