using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Interfaces;

namespace CloudCanvas.Shared.Services
{
    /// <summary>
    /// Provides a factory for creating and managing Service Bus clients for sending and receiving messages.
    /// </summary>
    /// <remarks>This class is designed to manage separate <see cref="ServiceBusClient"/> instances for
    /// sending and  receiving messages. It ensures proper disposal of the clients when the factory is disposed
    /// asynchronously.</remarks>
    public class SBClientFactory : IServiceBusClientFactory
    {
        private readonly ServiceBusClient _sendClient;
        private readonly ServiceBusClient _listenClient;

        public SBClientFactory(ServiceBusClient senderClient, ServiceBusClient listenerClient)
        {
            _sendClient = senderClient;
            _listenClient = listenerClient;
        }

        public ServiceBusClient GetListenClient() => _listenClient;
        public ServiceBusClient GetSendClient() => _sendClient;
    }
}
