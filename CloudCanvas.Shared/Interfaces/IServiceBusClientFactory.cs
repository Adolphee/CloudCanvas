using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Interfaces
{
    public interface IServiceBusClientFactory
    {
        public ServiceBusClient GetListenClient();
        public ServiceBusClient GetSendClient();
    }
}
