using Azure.Messaging.ServiceBus;
using CloudCanvas.Constants;
using CloudCanvas.Shared.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Services
{
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
