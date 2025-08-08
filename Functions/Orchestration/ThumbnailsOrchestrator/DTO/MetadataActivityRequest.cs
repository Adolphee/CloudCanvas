using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
namespace CloudCanvas.Functions.Orchestration.DTO
{
    public class MetadataActivityRequest: OrchestrationRequest
    {
        public MetadataActivityRequest(BlobMetaDTO blob, string correlationId, string instanceId) 
            : base(blob, correlationId, instanceId)
        {
        }
        public MetadataActivityRequest(BlobMetaDTO blob, OrchestrationRequest req, string instanceId) 
            : base(blob, req.CorrelationId, instanceId)
        {
        }
    }
}
