using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;

namespace CloudCanvas.Domain.Thumbnail
{
    public class PhotoThumbnail
    {
        public string? Id { get; set; }
        public ThumbnailSize Size { get; set; }
        public string Url { get; set; } = string.Empty;
        public required string? PostId { get; set; }
        public Photo OriginalPhoto { get; set; } = default!; 
        public string OriginalImageURL { get; set; } = default!;
    }
}