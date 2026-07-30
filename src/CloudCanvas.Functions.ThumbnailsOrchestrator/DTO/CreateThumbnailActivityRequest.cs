using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public record CreateThumbnailActivityRequest(PhotoDTO photo, ThumbnailSize thumbnailSize, string srcContainer, string correlationId, string instanceId)
        : ThumbnailOrchestrationRequest(photo, srcContainer, correlationId, instanceId)
    {
    }
}
