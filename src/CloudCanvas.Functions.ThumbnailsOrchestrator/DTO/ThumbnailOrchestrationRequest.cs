using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Infrastructure.DTOs;
namespace CloudCanvas.Functions.Orchestration.DTO
{
    public class ThumbnailOrchestrationRequest(BlobMetadata blob, string correlationId, string instanceId) : InceptionRequest(blob, correlationId)
    {
        public string InstanceId { get; set; } = instanceId;
    }

}
