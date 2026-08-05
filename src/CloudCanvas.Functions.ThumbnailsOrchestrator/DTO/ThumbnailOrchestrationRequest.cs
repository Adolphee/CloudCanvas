using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public record ThumbnailOrchestrationRequest(PhotoDTO photo, string srcContainer, string correlationId, string instanceId) 
        : InceptionRequest(photo, srcContainer, correlationId)
    {
    }
}
