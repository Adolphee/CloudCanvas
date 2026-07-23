using CloudCanvas.Application.Posts.DTOs;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public class InceptionRequest(PhotoDTO photo, string correlationId) : CorrelatedRequest(correlationId)
    {
        public PhotoDTO Photo { get; } = photo;
    }
}
