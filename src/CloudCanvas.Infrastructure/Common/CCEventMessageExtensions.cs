using Azure.Messaging.ServiceBus;
using CloudCanvas.Application.Common;
using CloudCanvas.Application.Events;
using CloudCanvas.Application.Posts.DTOs;

namespace CloudCanvas.Infrastructure.Common
{
    internal static class CCEventMessageExtensions
    {
        /// <summary>
        /// Converts `CCEventMessage` to `ServiceBusMessage`. 
        /// </summary>
        /// <returns>ServiceBusMessage</returns>
        internal static ServiceBusMessage ToSBMessage(this CCEventMessage message)
        {
            var msg = new ServiceBusMessage(message.Payload.ToString())
            {
                MessageId = Guid.NewGuid().ToString(),
                Subject = message.Subject,
                CorrelationId = message.CorrelationId
            };
            foreach (var prop in message.Properties)
                msg.ApplicationProperties[prop.Key] = prop.Value;
            return msg;
        }

        public static FileMetadata FromBinaryData(this CCSerializer serializer, BinaryData blobMetadataDto) => CCSerializer.Deserialize<FileMetadata>(blobMetadataDto.ToString());

    }
}
