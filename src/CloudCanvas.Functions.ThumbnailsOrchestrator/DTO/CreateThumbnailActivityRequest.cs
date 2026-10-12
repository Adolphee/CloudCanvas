using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public record CreateThumbnailActivityRequest(PhotoDTO Photo, ThumbnailSize thumbnailSize, string SrcContainer, string CorrelationId, string InstanceId)
        : ThumbnailOrchestrationRequest(Photo, SrcContainer, CorrelationId, InstanceId)
    {
    }
}
