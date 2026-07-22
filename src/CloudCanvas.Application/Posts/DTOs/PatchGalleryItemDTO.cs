using System.Text.Json.Serialization;

namespace CloudCanvas.Application.Posts.DTOs
{
    public class PatchGalleryItemDTO: MetadataDocumentBase
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("project")]
        public string? Project { get; set; }
        [JsonPropertyName("galleryName")]
        public string? GalleryName { get; set; }
        [JsonPropertyName("userTags")]
        public List<string>? UserTags { get; set; } = new();
    }
}
