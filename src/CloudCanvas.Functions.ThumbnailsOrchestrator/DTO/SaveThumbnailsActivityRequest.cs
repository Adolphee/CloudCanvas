using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Infrastructure.DTOs;
namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public sealed record SaveThumbnailsActivityRequest: ThumbnailOrchestrationRequest
    {
        public SaveThumbnailsActivityRequest(PhotoDTO photo, string srcContainer, string correlationId, string instanceId) 
            : base(photo, srcContainer, correlationId, instanceId)
        {
        }
    }
}
