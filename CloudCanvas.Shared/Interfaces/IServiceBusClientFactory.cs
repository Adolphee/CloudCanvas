using Azure.Messaging.ServiceBus;

namespace CloudCanvas.Shared.Interfaces
{
    /// <summary>
    /// Defines a factory for creating <see cref="ServiceBusClient"/> instances configured for specific messaging
    /// operations.
    /// </summary>
    /// <remarks>This interface provides methods to obtain <see cref="ServiceBusClient"/> instances tailored
    /// for listening to messages or sending messages. Implementations of this interface are responsible for managing
    /// the configuration and lifecycle of the clients.</remarks>
    public interface IServiceBusClientFactory
    {
        public ServiceBusClient GetListenClient();
        public ServiceBusClient GetSendClient();
    }
}
