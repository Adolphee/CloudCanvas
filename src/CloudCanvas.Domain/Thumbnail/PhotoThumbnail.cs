using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Domain.Thumbnail
{
    public class PhotoThumbnail: AuditableEntity
    {
        public string? Id { get; set; }
        public ThumbnailSize Size { get; set; }
        public required string Url { get; set; } = string.Empty;
        public required string? PhotoId { get; set; } = default!;
        public Photo OriginalPhoto { get; set; } = default!; 
        public string OriginalImageURL { get; set; } = default!;

        public KeyValuePair<string, string> ToKVPair() => new (Size.ToString(), Url);
    }
}