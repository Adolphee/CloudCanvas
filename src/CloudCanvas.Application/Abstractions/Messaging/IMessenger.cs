using CloudCanvas.Application.Events;
using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto;
using CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail;

namespace CloudCanvas.Application.Abstractions.Messaging
{
    /// <summary>
    /// Defines an adapter for interacting with a service bus, providing functionality to send messages to a specified
    /// topic.
    /// </summary>
    /// <remarks>This interface is designed to abstract the interaction with a service bus, enabling the
    /// sending of messages to specific topics. Implementations of this interface should handle the underlying service
    /// bus communication details.</remarks>
    public interface IMessenger
    {
        /// <summary>
        /// Publishes a single message to the specified Service Bus topic.
        /// </summary>
        /// <remarks>This method sends the specified message to the given topic.
        /// Ensure that the topic exists and is properly configured in the Service Bus namespace before calling
        /// this method.</remarks>
        /// <param name="topic">The name of the Service Bus topic to which the message will be published. Cannot be null or empty.</param>
        /// <param name="message">The <see cref="ServiceBusMessage"/> to be published. Cannot be null.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task<string> SendAsync(string topic, CCEventMessage message, CancellationToken cancellation = default);

        /// <summary>
        /// Publishes a batch of messages to the specified Service Bus topic.
        /// </summary>
        /// <remarks>This method publishes the provided batch of messages to the specified topic. If the
        /// <paramref name="batchCount"/> parameter is greater than 1, the message will be added to the batch multiple times. Ensure
        /// that the topic exists and is properly configured in the Service Bus namespace.</remarks>
        /// <param name="topic">The name of the Service Bus topic to which the messages will be published. Cannot be null or empty.</param>
        /// <param name="messages">A list of <see cref="ServiceBusMessage"/> objects to be published. Cannot be null or empty.</param>
        /// <param name="batchCount">The number of times the batch of messages should be published. Must be greater than or equal to 1. Defaults
        /// to 1.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task SendBatchAsync(string topic, List<CCEventMessage> messages, int batchCount = 1, CancellationToken cancellation = default);

        Task<string> NofityProjectionCompletedAsync(PhotoDTO photo, string correlationId, CancellationToken cancellation = default);
        Task<string> SendCreateThumbnailsCompletionMessage(PhotoDTO photo, string correlationId, CancellationToken cancellation = default);
    }
}
