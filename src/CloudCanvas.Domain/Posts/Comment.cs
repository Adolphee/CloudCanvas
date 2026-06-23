using CloudCanvas.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Domain.Posts
{
    public class Comment: Post
    {
        private readonly static PostClassification PostCategory = PostClassification.Comment;
        [MaxLength(255), Required]
        public string Text { get; set; } = default!;
        public string PostId { get; set; } = default!;

        public Post Post { get; set; } = default!;
    }

    // Idea: introduce Special (Rich Text) comments
    // Idea: introduce comment threads (replies to comments)
}