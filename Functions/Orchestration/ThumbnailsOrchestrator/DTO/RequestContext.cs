using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;

namespace CloudCanvas.Functions.Orchestration.DTOs
{
    public class RequestContext
    {
        public string InstanceId { get; set; } = default!;
        public string CorrelationId { get; set; } = default!;
        public BlobMetaDTO Blob { get; set; } = default!;
        public ThumbnailSize ThumbnailSize { get; set; }
    }
}
