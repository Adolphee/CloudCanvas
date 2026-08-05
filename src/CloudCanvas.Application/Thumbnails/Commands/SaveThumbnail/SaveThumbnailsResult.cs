using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Application.Thumbnails.Commands.SaveThumbnail
{
    public sealed record SaveThumbnailsResult(PhotoDTO? Photo)
    {
        public PhotoDTO? Photo { get; set; } = Photo;
    }
}
