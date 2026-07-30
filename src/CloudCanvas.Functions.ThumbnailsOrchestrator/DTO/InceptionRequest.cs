
namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public record InceptionRequest(PhotoDTO photo,string srcContainer, string correlationId) : CorrelatedRequest(correlationId)
    {
    }
}
