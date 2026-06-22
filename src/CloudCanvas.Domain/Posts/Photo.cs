using CloudCanvas.Domain.Posts.ValueObjects;
using CloudCanvas.Domain.Thumbnail;
using CloudCanvas.Infrastructure.Common;
using System.Text.Json.Serialization;

namespace CloudCanvas.Domain.Posts
{
    public class Photo: Post
    {
        private readonly static PostClassification PostCategory = PostClassification.Photo;
        public string? Title { get;  set; }
        public string OriginalFilename { get; set; } = default!;
        public List<PhotoThumbnail> Thumbnails { get; set; } = new(); // BlobUrls for the thumbnails
        public List<string> UserTags { get; set; } = new();
        public Photo() { }
    }
}