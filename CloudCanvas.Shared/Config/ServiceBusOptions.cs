using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Shared.Config
{
    public class ServiceBusOptions
    {
        public Dictionary<string, Topic> Topics { get; set; }
    }
}
