using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public record ThumbnailOrchestrationRequest(PhotoDTO Photo, string SrcContainer, string CorrelationId, string InstanceId) 
        : InceptionRequest(Photo, SrcContainer, CorrelationId)
    {
    }
}
