using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Interfaces;
using Microsoft.Extensions.Configuration;
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

        public ServiceBusAdapter(ServiceBusClientFactory factory, ILogger<ServiceBusAdapter> logger, IConfiguration config)
        {
            _logger = logger;
            _factory = factory;
        }

        public async Task SendAsync(string topic, ServiceBusMessage message)
        {
            
            var sender = _factory.GetSendClient().CreateSender(topic);
            try
            {
                await sender.SendMessageAsync(message);
                _logger.LogInformation($"Message has been published to topic: {topic}.");
            } catch(ServiceBusException e)
            {
                _logger.LogWarning($"The messages \"{message.MessageId}\" failed to send:");
                _logger.LogWarning($"{nameof(ServiceBusException)}: {e.Message}");
                _logger.LogDebug($"Received message: {message.ToString()}");
                _logger.LogDebug($"{nameof(ServiceBusException)} (StackTrace): {e.StackTrace}");
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }


        public async Task SendBatchAsync(string topic, List<ServiceBusMessage> messages, int count = 1)
        {
            var sender = _factory.GetSendClient().CreateSender(topic);
            using var messageBatch = await sender.CreateMessageBatchAsync();

            foreach (var message in messages)
            {
                if (!messageBatch.TryAddMessage(message))
                {
                    var msg = $"The messages \"{message.ToString()}\" is too large to fit in the batch.";
                    _logger.LogDebug(msg);
                    throw new Exception(msg);
                }
            }

            try
            {
                await sender.SendMessagesAsync(messageBatch);
                _logger.LogInformation($"Batch of {count} messages has been published to topic: {topic}.");
            }
            catch (Exception e)
            {
                var msg = $"The messages \"{messages.ToString()}\" failed to send.";
                _logger.LogDebug(msg, e);
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }

    }
}
