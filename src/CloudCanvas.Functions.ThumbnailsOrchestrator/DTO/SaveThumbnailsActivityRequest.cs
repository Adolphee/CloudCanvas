using CloudCanvas.Application.Posts.Photos;
using CloudCanvas.Infrastructure.DTOs;
namespace CloudCanvas.Functions.ThumbnailOrchestrator.DTO
{
    public sealed record SaveThumbnailsActivityRequest: ThumbnailOrchestrationRequest
    {
        public SaveThumbnailsActivityRequest(PhotoDTO Photo, string SrcContainer, string CorrelationId, string InstanceId) 
            : base(Photo, SrcContainer, CorrelationId, InstanceId)
        {
        }
    }
}
