using CloudCanvas.Domain.Posts.ValueObjects;
using CloudCanvas.Domain.Thumbnail;

namespace CloudCanvas.Domain.Posts
{
    public class Photo: Post
    {
        private readonly static PostCategory PostCategory = PostCategory.Photo;
        public string? Title { get;  set; }
        public string OriginalFilename { get; set; } = default!;
        public List<PhotoThumbnail> Thumbnails { get; set; } = new(); // BlobUrls for the thumbnails
        public List<string> UserTags { get; set; } = new();
        public Photo() { }
    }
}