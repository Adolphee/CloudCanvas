using CloudCanvas.Domain.Posts.ValueObjects;
using CloudCanvas.Infrastructure.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CloudCanvas.Domain.Posts
{
    public class Comment: Post
    {
        private readonly static PostClassification PostCategory = PostClassification.Comment;
        [MaxLength(255), Required]
        public string Text { get; set; } = default!;
        public string PostId { get; set; } = default!;

        //[JsonConverter(typeof(PostJsonConverter))]
        public Post Post { get; set; } = default!;
    }

    // Idea: introduce Special (Rich Text) comments
    // Idea: introduce comment threads (replies to comments)
}