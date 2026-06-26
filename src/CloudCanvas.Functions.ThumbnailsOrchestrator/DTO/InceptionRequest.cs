using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Infrastructure.DTOs;

namespace CloudCanvas.Functions.Orchestration.DTO
{
    public class InceptionRequest(BlobMetadata blob, string correlationId) : CorrelatedRequest(correlationId)
    {
        public BlobMetadata Blob { get; } = blob;
    }
}
