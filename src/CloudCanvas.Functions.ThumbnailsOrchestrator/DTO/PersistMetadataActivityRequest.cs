using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Infrastructure.DTOs;
namespace CloudCanvas.Functions.Orchestration.DTO
{
    public class PersistMetadataActivityRequest: ThumbnailOrchestrationRequest
    {
        public PersistMetadataActivityRequest(BlobMetadata blob, string correlationId, string instanceId) 
            : base(blob, correlationId, instanceId)
        {
        }
        public PersistMetadataActivityRequest(BlobMetadata blob, ThumbnailOrchestrationRequest req, string instanceId) 
            : base(blob, req.CorrelationId, instanceId)
        {
        }
    }
}
