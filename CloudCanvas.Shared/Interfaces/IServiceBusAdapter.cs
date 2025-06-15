using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Interfaces
{
    public interface IServiceBusAdapter
    {
        public Task SendAsync(string topic, ServiceBusMessage message, int count);
        // public Task ProcessIncomingMessagesAsync(string topic);
    }
}
