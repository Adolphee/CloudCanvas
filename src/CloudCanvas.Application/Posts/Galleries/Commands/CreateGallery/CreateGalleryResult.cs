using CloudCanvas.Application.Common;

namespace CloudCanvas.Application.Posts.Galleries.Commands.CreateGallery
{
    public record CreateGalleryResult
    {
        public CCOperationStatus Status = CCOperationStatus.Success;
        public GalleryDTO Gallery { get; set; } = default!;
    }
}
