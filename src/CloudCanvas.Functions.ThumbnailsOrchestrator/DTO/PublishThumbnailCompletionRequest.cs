using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public sealed record PublishThumbnailCompletionRequest(PhotoDTO Photo, string CorrelationId, string InstanceId) 
        : ThumbnailOrchestrationRequest(Photo, default!, CorrelationId, InstanceId)
    {
    }
}
