using Azure.Messaging.ServiceBus;
using CloudCanvas.Shared.Constants;
using CloudCanvas.Shared.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CloudCanvas.Shared.Services
{
    /// <summary>
    /// Provides a factory for creating and managing <see cref="ServiceBusClient"/> instances  for sending and receiving
    /// messages in Azure Service Bus.
    /// </summary>
    /// <remarks>This factory initializes and manages two <see cref="ServiceBusClient"/> instances: one for
    /// sending messages and another for receiving messages. The connection strings for these clients are retrieved from
    /// the provided <see cref="IConfiguration"/> instance.</remarks>
    public class ServiceBusClientFactory : IServiceBusClientFactory, IAsyncDisposable
    {
        private ServiceBusClient _sendClient;
        private ServiceBusClient _listenClient;
        private readonly IConfiguration _config;

        public ServiceBusClientFactory(IConfiguration config)
        {
            _config = config;
            _listenClient = new ServiceBusClient(config.GetConnectionString(ServiceBus.Topics.FileUpdate.Listen));
            _sendClient = new ServiceBusClient(config.GetConnectionString(ServiceBus.Topics.FileUpdate.Send));
        }

        public ServiceBusClient GetListenClient() => _listenClient;
        public ServiceBusClient GetSendClient() => _sendClient;

        public async ValueTask DisposeAsync()
        {
            await _sendClient.DisposeAsync();
            await _listenClient.DisposeAsync();
        }
    }
}
