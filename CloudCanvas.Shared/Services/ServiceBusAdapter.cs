using Azure.Messaging.ServiceBus;
using CloudCanvas.Constants;
using CloudCanvas.Services;
using CloudCanvas.Shared.Config;
using CloudCanvas.Shared.Interfaces;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Services
{
    public class ServiceBusAdapter : IServiceBusAdapter
    {
        private readonly ILogger<ServiceBusAdapter> _logger;
        private readonly IConfiguration _config;
        private readonly IOptions<ServiceBusOptions> _options;
        private readonly ServiceBusClientFactory _factory;

        public ServiceBusAdapter(ServiceBusClientFactory factory, IOptions<ServiceBusOptions> options, ILogger<ServiceBusAdapter> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _options = options;
            _factory = factory;
        }

        public async Task SendAsync(string topic, ServiceBusMessage message, int count = 1)
        {
            var sender = _factory.GetSendClient().CreateSender(topic);
            using var messageBatch = await sender.CreateMessageBatchAsync();

            for (var i = 0; i < count; i++)
            {
                if (!messageBatch.TryAddMessage(message))
                {
                    var msg = $"The message \"{message.ToString()}\" is too large to fit in the batch.";
                    _logger.LogDebug(msg);
                    throw new Exception(msg);
                }
            }

            try
            {
                await sender.SendMessagesAsync(messageBatch);
                _logger.LogInformation($"Batch of {count} messages has been published to topic: {topic}.");
            } catch(Exception e)
            {
                var msg = $"The message \"{message.ToString()}\" failed to send.";
                _logger.LogDebug(msg);
            }
            finally
            {
                await sender.DisposeAsync();
            }
        }
    }
}
