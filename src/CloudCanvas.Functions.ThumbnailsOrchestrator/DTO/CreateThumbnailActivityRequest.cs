using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public class CreateThumbnailActivityRequest: ThumbnailOrchestrationRequest
    {
        public CreateThumbnailActivityRequest(PhotoDTO photo, ThumbnailSize size, string correlationId, string instanceId)
            : base(photo, correlationId, instanceId)
        {
        }
    }
}
