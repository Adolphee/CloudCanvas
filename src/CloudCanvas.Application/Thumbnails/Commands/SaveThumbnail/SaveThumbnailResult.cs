using CloudCanvas.Application.Common;
using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    public sealed record SaveThumbnailResult
    {
        public CCOperationStatus Status { get; set; } = CCOperationStatus.Failed;
        public PhotoDTO Photo { get; set; } = default!;
        public ThumbnailSize Size { get; set; }
        public string? Location { get; set; } = null;
    }
}
