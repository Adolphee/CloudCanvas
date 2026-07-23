using CloudCanvas.Application.Posts.DTOs;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public sealed class PublishCompletionRequest(PhotoDTO Photo, string correlationId, string instanceId) 
        : SaveThumbnailsActivityRequest(Photo, correlationId, instanceId)
    {
        public PublishCompletionRequest(CreateThumbnailActivityRequest req)
            : this(req.Photo, req.CorrelationId, req.InstanceId) { }
    }
}
