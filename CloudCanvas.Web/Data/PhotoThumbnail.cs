using CloudCanvas.Shared.Enums;

namespace CloudCanvas.Web.Data
{
    public class PhotoThumbnail
    {
        public string? Id { get; set; }
        public ThumbnailSize Size { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? PostId { get; set; }
        public Photo OriginalPhoto { get; set; } = default!; 
        public string OriginalImageURL { get; set; } = default!;
    }
}