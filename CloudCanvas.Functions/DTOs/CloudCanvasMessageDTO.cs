using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Functions.DTOs
{
    public class CloudCanvasMessageDTO
    {
        public string Event { get; set; }
        public string Subject { get; set; }
        public BlobMetaDTO Payload { get; set; }
    }
}
