using CloudCanvas.Application.Common;

namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    public sealed record SaveThumbnailsResult
    {
        public CCOperationStatus Status { get; set; } = CCOperationStatus.Failed;
        public PhotoDTO? Photo { get; set; } = default!;
    }
}
