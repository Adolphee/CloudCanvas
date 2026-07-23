using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Infrastructure.DTOs;
namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public class SaveThumbnailsActivityRequest: ThumbnailOrchestrationRequest
    {
        public SaveThumbnailsActivityRequest(PhotoDTO blob, string correlationId, string instanceId) 
            : base(blob, correlationId, instanceId)
        {
        }
        public SaveThumbnailsActivityRequest(PhotoDTO blob, ThumbnailOrchestrationRequest req, string instanceId) 
            : base(blob, req.CorrelationId, instanceId)
        {
        }
    }
}
