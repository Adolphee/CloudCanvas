using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Infrastructure.Messaging
{
    public static class MessageFactory
    {
        public static IServiceBusMessageBuilder BuildFor(object payload)
        {
            return new SBMessageBuilder(payload);
        }
    }
}
