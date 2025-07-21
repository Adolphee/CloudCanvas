using CloudCanvas.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Services
{
    public static class MessageFactory
    {
        public static IServiceBusMessageBuilder BuildFor(object payload)
        {
            return new SBMessageBuilder(payload);
        }
    }
}
