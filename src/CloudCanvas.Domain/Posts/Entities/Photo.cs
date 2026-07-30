using CloudCanvas.Domain.Thumbnail;

namespace CloudCanvas.Domain.Posts.Entities
{
    public class Photo: Post
    {
        public string? Title { get;  set; }
        public string OriginalFilename { get; set; } = default!;
        public string? Caption { get; set; } = default!;
        public string? GalleryId { get; set; } = default!;
        public Gallery? Gallery { get; set; } = default!;
        public List<PhotoThumbnail> Thumbnails { get; set; } = [];
    }
}