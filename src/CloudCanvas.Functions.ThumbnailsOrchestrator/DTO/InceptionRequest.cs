
using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public record InceptionRequest(PhotoDTO Photo,string SrcContainer, string CorrelationId) : CorrelatedRequest(CorrelationId)
    {
    }
}
