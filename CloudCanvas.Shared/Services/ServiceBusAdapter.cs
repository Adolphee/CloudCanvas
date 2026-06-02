using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Exceptions;
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
        private readonly IServiceBusClientFactory _factory;

        public ServiceBusAdapter(IServiceBusClientFactory factory, ILogger<ServiceBusAdapter> logger)
        {
            _logger = logger;
            _factory = factory;
        }

        /// <summary>
        /// Sends a message to the specified Service Bus topic asynchronously.
        /// </summary>
        /// <remarks>This method creates a sender for the specified topic, sends the provided message, and
        /// ensures proper disposal of the sender. If the operation fails, a <see cref="ServiceBusException"/> is logged
        /// and rethrown.</remarks>
        /// <param name="topic">The name of the Service Bus topic to which the message will be sent. Cannot be null or empty.</param>
        /// <param name="msg">The <see cref="ServiceBusMessage"/> to send. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous send operation.</returns>
        public async Task<string> SendAsync(string topic, ServiceBusMessage msg)
        {
            var sender = _factory.GetSendClient(topic);
            try
            {
                await sender.SendMessageAsync(msg);
                return msg.MessageId;
            } catch(ServiceBusException e)
            {
                _logger.LogError(e, "Failed to send message '{messageId}' to topic: '{topic}'.", msg.MessageId, topic);
                throw;
            }
        }

        /// <summary>
        /// Sends a batch of messages to the specified Service Bus topic.
        /// </summary>
        /// <remarks>This method creates a message batch and attempts to add the provided messages to it. 
        /// If a message cannot be added due to the batch size limit, a <see cref="MessageBatchFullException"/> is
        /// thrown. After successfully adding messages to the batch, the method sends the batch to the specified
        /// topic.</remarks>
        /// <param name="topic">The name of the Service Bus topic to which the messages will be sent.</param>
        /// <param name="messages">A list of <see cref="ServiceBusMessage"/> instances to be included in the batch.</param>
        /// <param name="maxBatchSize">The maximum number of messages allowed in a single batch. Defaults to 1.  If the batch exceeds this size, an
        /// exception will be thrown.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="MessageBatchFullException">Thrown if a message cannot be added to the batch because the batch size exceeds the specified <paramref
        /// name="maxBatchSize"/>.</exception>
        public async Task SendBatchAsync(string topic, List<ServiceBusMessage> messages, int maxBatchSize = 1)
        {
            var sender = _factory.GetSendClient(topic);
            using var messageBatch = await sender.CreateMessageBatchAsync();

            foreach (var message in messages)
            {
                try { messageBatch.TryAddMessage(message); }
                catch (Exception e)
                {
                    _logger.LogError("Failed to add message {messageId} to the batch (batchSize: {batchCount}, max: {maxBatchSize}).", message.MessageId, messageBatch.Count, maxBatchSize);
                    throw new MessageBatchFullException($"Failed to add message {message.MessageId} to the batch (batchSize: {messageBatch.Count}, max: {maxBatchSize}).", e);
                }
            }

            try { await sender.SendMessagesAsync(messageBatch); }
            catch (ServiceBusException e)
            {
                _logger.LogError(e, "MessageBatch (count: {batchSize}) failed to send.", messageBatch.Count);
                throw;
            }
        }

    }
}
