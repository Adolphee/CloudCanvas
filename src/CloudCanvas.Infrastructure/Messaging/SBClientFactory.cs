using Azure.Messaging.ServiceBus;
using CloudCanvas.Application.Abstractions.Messaging;

namespace CloudCanvas.Infrastructure.Messaging
{
    /// <summary>
    /// Provides a factory for creating and managing Service Bus clients for sending and receiving messages.
    /// </summary>
    /// <remarks>This class is designed to manage separate <see cref="ServiceBusClient"/> instances for
    /// sending and  receiving messages. It ensures proper disposal of the clients when the factory is disposed
    /// asynchronously.</remarks>
    public class SBClientFactory : IMessengerFactory
    {
        private readonly ServiceBusClient _client;
        private ServiceBusSender? _sendClient;
        private ServiceBusReceiver? _listenClient;

        public SBClientFactory(ServiceBusClient client)
        {
            _client = client;
        }

        public ServiceBusSender GetSender(string topic)
        {
            if(_sendClient == null)
                _sendClient = _client.CreateSender(topic);
            return _sendClient;
        }

        public ServiceBusReceiver GetReceiver(string topic)
        {
            if(_listenClient == null)
                _listenClient = _client.CreateReceiver(topic);
            return _listenClient;
        }
    }
}
