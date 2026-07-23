using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Infrastructure.DTOs;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public class RequestContext
    {
        public string InstanceId { get; set; } = default!;
        public string CorrelationId { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
        public PhotoDTO Photo { get; set; } = default!;
        public ThumbnailSize ThumbnailSize { get; set; }
        public string ThumbnailURL { get; set; } = default!;
    }
}
