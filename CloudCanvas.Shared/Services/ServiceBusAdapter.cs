using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Shared.Services
{
    /// <summary>
    /// Provides functionality for sending messages to Azure Service Bus topics.
    /// </summary>
    /// <remarks>This adapter simplifies the process of sending messages to Azure Service Bus by managing the
    /// creation of messages batches and handling logging for successful and failed operations.</remarks>
    public class ServiceBusAdapter : IServiceBusAdapter
    {
        private readonly ILogger<ServiceBusAdapter> _logger;
        private readonly ServiceBusClientFactory _factory;

        public ServiceBusAdapter(ServiceBusClientFactory factory, ILogger<ServiceBusAdapter> logger)
        {
            _logger = logger;
            _factory = factory;
        }

        public async Task SendAsync(string topic, ServiceBusMessage msg)
        {
            
            var sender = _factory.GetSendClient().CreateSender(topic);
            try
            {
                await sender.SendMessageAsync(msg);
                _logger.LogInformation("Message {messageId} has been published to topic: {topic}.", msg.MessageId, topic);
            } catch(ServiceBusException e)
            {
                _logger.LogWarning(e, "Failed to send message '{messageId}' to topic: '{topic}'.", msg.MessageId, topic);
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }

        public async Task SendBatchAsync(string topic, List<ServiceBusMessage> messages, int maxBatchSize = 1)
        {
            var sender = _factory.GetSendClient().CreateSender(topic);
            using var messageBatch = await sender.CreateMessageBatchAsync();

            foreach (var message in messages)
            {
                if (!messageBatch.TryAddMessage(message))
                {
                    _logger.LogWarning("The messages {messageId} is too large to fit in the batch.", message.MessageId);
                    throw new OverflowException($"The message {message.MessageId} is too large to fit in the batch.");
                }
            }

            try
            {
                await sender.SendMessagesAsync(messageBatch);
                _logger.LogInformation("Batch of {successful}/{maxBatchSize} messages has been published to topic: {topic}.", messageBatch.Count, maxBatchSize, topic);
            }
            catch (ServiceBusException e)
            {
                _logger.LogError(e, "MessageBatch (count: {batchSize}) failed to send.", messageBatch.Count);
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }

    }
}
