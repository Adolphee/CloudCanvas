using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Application.Thumbnails.Commands.CreateThumbnail
{
    public sealed record CreateThumbnailResult(ThumbnailSize size, string thumbnailUrl, PhotoDTO originalPhoto)
    {
        public ThumbnailSize Size { get; set; } = size;
        public string ThumbnailUrl { get; set; } = thumbnailUrl;
        public PhotoDTO OriginalPhoto { get; set; } = originalPhoto;
    }
}