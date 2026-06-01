using CloudCanvas.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudCanvas.Functions.Orchestration.DTO
{
    public class PublishCompletionRequest(BlobMetaDTO blob, string correlationId, string instanceId) 
        : MetadataActivityRequest(blob, correlationId, instanceId)
    {
        public PublishCompletionRequest(ThumbnailActivityRequest req)
            : this(req.Blob, req.CorrelationId, req.InstanceId) { }
    }
}
