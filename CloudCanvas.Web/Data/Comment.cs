using CloudCanvas.Web.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Web.Data
{
    public class Comment: Post
    {
        [MaxLength(255), Required]
        public string Text { get; set; } = default!;
        public string PostId { get; set; } = default!;
        public Post Post { get; set; } = default!;
    }

    // Idea: introduce Special (Rich Text) comments
    // Idea: introduce comment threads (replies to comments)
}