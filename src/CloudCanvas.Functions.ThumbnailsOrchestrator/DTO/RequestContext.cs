using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public class RequestContext
    {
        public string InstanceId { get; set; } = default!;
        public string CorrelationId { get; set; } = default!;
        public PhotoDTO Photo { get; set; } = default!;
    }
}
