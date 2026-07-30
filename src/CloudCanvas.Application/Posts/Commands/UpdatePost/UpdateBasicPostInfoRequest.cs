using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CloudCanvas.Application.Posts.Commands.UpdatePost
{
    public record UpdateBasicPostInfoRequest
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; init; } = default!;

        [Required]
        [JsonPropertyName("userId")]
        public string? UserId { get; init; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; init; }

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("galleryName")]
        public string? GalleryName { get; init; }

        [JsonPropertyName("userTags")]
        public List<string>? UserTags { get; init; } = new();
        
        [JsonPropertyName("galleryId")]
        public string? GalleryId { get; init; }

        [JsonPropertyName("commentsEnabled")]
        public bool? CommentsEnabled { get; init; } = true;

        [JsonPropertyName("published")]
        public bool? IsPublished { get; init; }

    }
}
