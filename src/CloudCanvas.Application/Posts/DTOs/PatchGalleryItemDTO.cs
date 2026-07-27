using System.Text.Json.Serialization;

namespace CloudCanvas.Application.Posts.DTOs
{
    public record PatchGalleryItemDTO: MetadataDocumentBase
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; init; }
        [JsonPropertyName("description")]
        public string? Description { get; init; }
        [JsonPropertyName("project")]
        public string? Project { get; init; }
        [JsonPropertyName("galleryName")]
        public string? GalleryName { get; init; }
        [JsonPropertyName("userTags")]
        public List<string>? UserTags { get; init; } = new();
    }
}
