using Azure.Messaging.ServiceBus;
using CloudCanvas.Application.Abstractions.Messaging;
using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Events;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail;
using CloudCanvas.Infrastructure.Common;
using CloudCanvas.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Infrastructure.Messaging
{
    /// <summary>
    /// Provides functionality for sending messages to Azure Service Bus topics.
    /// </summary>
    /// <remarks>This adapter simplifies the process of sending messages to Azure Service Bus by managing the
    /// creation of messages batches and handling logging for successful and failed operations.</remarks>
    public class Messenger(IMessengerFactory factory, ILogger<Messenger> logger, IMessageFactory mFactory) : IMessenger
    {
        private readonly ILogger<Messenger> _logger = logger;
        private readonly IMessengerFactory _factory = factory;
        private readonly IMessageFactory _mFactory = mFactory;

        /// <summary>
        /// Sends a message to the specified Service Bus topic asynchronously.
        /// </summary>
        /// <remarks>This method creates a sender for the specified topic, sends the provided message, and
        /// ensures proper disposal of the sender. If the operation fails, a <see cref="ServiceBusException"/> is logged
        /// and rethrown.</remarks>
        /// <param name="topic">The name of the Service Bus topic to which the message will be sent. Cannot be null or empty.</param>
        /// <param name="msg">The <see cref="ServiceBusMessage"/> to send. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous send operation.</returns>
        public async Task<string> SendAsync(string topic, CCEventMessage msg, CancellationToken cancellation = default)
        {
            var sender = _factory.GetSender(topic);
            ServiceBusMessage message = msg.ToSBMessage();
            try
            {
                await sender.SendMessageAsync(message, cancellation);
                return message.MessageId;
            } catch(ServiceBusException e)
            {
                _logger.LogError(e, "Failed to send message '{messageId}' to topic: '{topic}'.", message.MessageId, topic);
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
        public async Task SendBatchAsync(string topic, List<CCEventMessage> messages, int maxBatchSize = 1, CancellationToken cancellation = default)
        {
            var sender = _factory.GetSender(topic);
            using var messageBatch = await sender.CreateMessageBatchAsync(cancellation);

            foreach (var msg in messages)
            {
                ServiceBusMessage message = msg.ToSBMessage();
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

        public async Task<string> NofityProjectionCompletedAsync(string srcContainer, PhotoDTO photo, string correlationId, CancellationToken cancellation = default)
        {
            var msg = _mFactory.BuildForPhoto(photo).ProjectionCompleteMessage(srcContainer, correlationId).Finalize();
            return await SendAsync(ServiceBus.Topics.FileUpdates, msg, cancellation);
        }

        public async Task<string> SendCreateThumbnailsCompletionMessage(PhotoDTO photo, string correlationId, CancellationToken cancellation = default)
        {
            var msg = _mFactory.BuildForPhoto(photo).ThumbnailsCreatedMessage(correlationId).Finalize();
            return await SendAsync(ServiceBus.Topics.FileUpdates, msg, cancellation);
        }
    }
}
