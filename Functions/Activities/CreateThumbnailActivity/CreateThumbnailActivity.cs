using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Services;
using Google.Protobuf.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;

namespace CreateThumbnailActivity
{
    // You will have noticed by now that there are "new" key words in many places in this function
    // that's because in-proces functions do not support Dependency Injection to the same extent as isolated functions
    // durable orchestration was the only reason why I even wanted to try in-process functions, but it didn't work out
    public class CreateThumbnailActivity
    {
        [FunctionName("CreateThumbnailActivity")]
        public async Task Run([ActivityTrigger] BlobMetaDTO blobmeta, ILogger logger)
        {
            const string thumbnails = BlobStorage.Containers.Thumbnails;
            const string uploads = BlobStorage.Containers.Uploads;
            var converter = new BlobMetaConverter();
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            try
            {
                ///TODO: Implement **better validation** on file type, 
                /// for example, what if this function receives a .pdf file? or a .mp4, .zip etc...
                var blobService = new BlobStorageService(config, (ILogger<BlobStorageService>) logger);
                var bclient = await blobService.GetContainerClientAsync(uploads); // original file blob container
                var stream = await bclient.GetBlobClient(blobmeta.OriginalFileName).OpenReadAsync(); // download file
                bclient = await blobService.GetContainerClientAsync(thumbnails); // switch to thumbnails desination container
                using var image = await Image.LoadAsync(stream);
                image.Mutate(i => i.Resize(50, 50)); // In the real world, more complex operations can be performed in this step
                using var output = new MemoryStream();
                await image.SaveAsJpegAsync(output);
                output.Position = 0;
                await blobService.UploadAsync(thumbnails, output, blobmeta.OriginalFileName); //upload the thumbnail
                logger.LogInformation($"Successfully created thumbnail and saved to [{thumbnails}]/{blobmeta.OriginalFileName}.");
            }
            catch (Exception e)
            {
                logger.LogError($"Failed to Create Thumbnail. Service Bus MessageBody [{ServiceBus.Subs.CreateThumbnail}]:\n{blobmeta.ToString()}");
                logger.LogDebug(e.Message, e.StackTrace); 
                // TODO: implement retry policies
            }
            var body = new ExtractMetadataMessageDTO
            {
                Event = ServiceBus.GetRealEventString(ServiceBus.Topics.FileUpdates, ServiceBus.Subs.CreateThumbnail, "done"),
                Subject = "Thumbnail Created.",
                Payload = blobmeta
            };

            var responseMessage = new ServiceBusMessage(converter.Serialize(body));
            // Tweaking so the message goes through subscription filters on function CreateThumbnail
            responseMessage.ApplicationProperties.Add(ServiceBus.Props.EventType, ServiceBus.Subs.CreateThumbnail);
            // I am forced to create to call for a client and send the message manually,
            // because for dotnet-isolated functions there is no IAsyncCollector<ServiceBusMessage> I can call
            // to set ApplicationProperties on the message, which I MUST to do for subscription filtering (example: persist-metadata) to work
            var sbAdapter = new ServiceBusAdapter(new ServiceBusClientFactory(config), (ILogger<ServiceBusAdapter>) logger, config);
            await sbAdapter.SendAsync(ServiceBus.Topics.FileUpdates, responseMessage);
            logger.LogInformation($"C# Blob trigger function Processed blob\n Name:{blobmeta.OriginalFileName} \n Size: {blobmeta.OriginalFileName.Length} Bytes");
        }
    }
}
