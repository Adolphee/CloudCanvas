using CloudCanvas.Shared.DTOs;
using CloudCanvas.Shared.Enums;
using CloudCanvas.Web.Interfaces;
using Microsoft.Azure.Cosmos;

namespace CloudCanvas.Web.Data
{
    public class Photo: Post
    {
        public string? Title { get;  set; }
        public string OriginalFilename { get; set; } = default!;
        public List<PhotoThumbnail> Thumbnails { get; set; } = new(); // BlobUrls for the thumbnails
        public List<string> UserTags { get; set; } = new();
        public Photo() { }
    }
}