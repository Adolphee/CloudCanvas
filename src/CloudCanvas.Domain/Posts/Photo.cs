using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Thumbnail;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Domain.Posts
{
    public record class Photo: Post
    {
        private readonly static PostClassification PostCategory = PostClassification.Photo;
        public string? Title { get;  set; }
        public string OriginalFilename { get; set; } = default!;
        public string? Caption { get; set; } = default!;
        [NotMapped]
        public string? GalleryId { get; set; } = default!;
        public Gallery? Gallery { get; set; } = default!;
        public List<PhotoThumbnail> Thumbnails { get; set; } = new();
        public Photo() { }
    }
}