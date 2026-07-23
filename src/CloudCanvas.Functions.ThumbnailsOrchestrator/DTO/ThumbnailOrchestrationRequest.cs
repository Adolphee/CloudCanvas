using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Infrastructure.DTOs;
namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public class ThumbnailOrchestrationRequest(PhotoDTO blob, string correlationId, string instanceId) : InceptionRequest(blob, correlationId)
    {
        public string InstanceId { get; set; } = instanceId;
    }

}
