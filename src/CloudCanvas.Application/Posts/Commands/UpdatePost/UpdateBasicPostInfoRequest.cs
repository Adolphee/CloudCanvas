using CloudCanvas.Application.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CloudCanvas.Application.Posts.Commands.UpdatePost
{
    public class UpdateBasicPostInfoRequest
    {
        [Required]
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [Required]
        [JsonPropertyName("userId")]
        public string? UserId { get; set; } // should be overwritten for more structured Id's

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("galleryName")]
        public string? GalleryName { get; set; }

        [JsonPropertyName("userTags")]
        public List<string>? UserTags { get; set; } = new();
        
        [JsonPropertyName("galleryId")]
        public string? GalleryId { get; set; }

        [JsonPropertyName("commentsEnabled")]
        public bool? CommentsEnabled { get; set; } = true;

        [JsonPropertyName("published")]
        public bool? IsPublished { get; set; }

    }
}
