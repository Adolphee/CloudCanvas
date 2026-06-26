
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Infrastructure.DTOs;

namespace CloudCanvas.Functions.Orchestration.DTO
{
    public class ThumbnailActivityRequest: ThumbnailOrchestrationRequest
    {
        public ThumbnailSize ThumbnailSize { get; set; } = ThumbnailSize.small;
        public ThumbnailActivityRequest(BlobMetadata blob, string correlationId, string instanceId)
            : base(blob, correlationId, instanceId)
        {
        }
    }
}
