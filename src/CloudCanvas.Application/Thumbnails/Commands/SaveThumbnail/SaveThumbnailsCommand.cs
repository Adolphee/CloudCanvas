using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    public sealed record SaveThumbnailsCommand: IRequest<SaveThumbnailsResult>
    {
        public required PhotoDTO Photo { get; init; } = default!;
        public required Creator Creator { get; init; } = default!;
    }
}
