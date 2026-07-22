using Azure.Messaging.ServiceBus;

namespace CloudCanvas.Application.Abstractions.Messaging
{
    /// <summary>
    /// Defines a factory for creating <see cref="ServiceBusClient"/> instances configured for specific messaging
    /// operations.
    /// </summary>
    /// <remarks>This interface provides methods to obtain <see cref="ServiceBusClient"/> instances tailored
    /// for listening to messages or sending messages. Implementations of this interface are responsible for managing
    /// the configuration and lifecycle of the clients.</remarks>
    public interface IMessengerFactory
    {
        public ServiceBusSender GetSender(string topic);
        public ServiceBusReceiver GetReceiver(string topic);
    }
}
