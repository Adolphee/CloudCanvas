using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;

namespace CloudCanvas.Functions.Orchestration.DTO
{
    public class ThumbnailActivityRequest: OrchestrationRequest
    {
        public ThumbnailSize ThumbnailSize { get; set; } = ThumbnailSize.small;
        public ThumbnailActivityRequest(BlobMetaDTO blob, string correlationId, string instanceId)
            : base(blob, correlationId, instanceId)
        {
        }
    }
}
