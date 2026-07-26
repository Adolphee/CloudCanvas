using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    public record SaveThumbnailsCommand: IRequest<SaveThumbnailsResult>
    {
        public PhotoDTO Photo { get; set; } = default!;
        public Creator creator { get; set; } = default!;
        public string OriginalContainer { get; set; } = default!;
    }
}
