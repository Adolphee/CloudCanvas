using CloudCanvas.Infrastructure.DTOs;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public class InceptionRequest(BlobMetadata blob, string correlationId) : CorrelatedRequest(correlationId)
    {
        public BlobMetadata Blob { get; } = blob;
    }
}
