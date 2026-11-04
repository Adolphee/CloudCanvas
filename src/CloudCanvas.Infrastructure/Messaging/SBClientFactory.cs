using Azure.Messaging.ServiceBus;

namespace CloudCanvas.Infrastructure.Messaging
{
    /// <summary>
    /// Provides a factory for creating and managing Service Bus clients for sending and receiving messages.
    /// </summary>
    public sealed class SBClientFactory : IMessengerFactory
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
            _sendClient ??= _client.CreateSender(topic);
            return _sendClient;
        }

        public ServiceBusReceiver GetReceiver(string topic)
        {
            _listenClient ??= _client.CreateReceiver(topic);
            return _listenClient;
        }
    }
}
