using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    public record SaveThumbnailCommand: IRequest<SaveThumbnailResult>
    {
        public PhotoDTO Photo { get; set; } = default!;
        public ThumbnailSize ThumbnailSize { get; set; }
        public string ThumbnailURL { get; set; } = default!;
        public Creator creator { get; set; } = default!;
    }
}
