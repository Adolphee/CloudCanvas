
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudCanvas.Domain.Entities;
namespace CloudCanvas.Domain.Posts.Entities
{
    public class Comment : ReactionTarget
    {
        [MaxLength(255), Required]
        public required string Text { get; set; } = default!;
        [Required]
        public required string PostId { get; init; } = default!;
        public Post Post { get; set; } = default!;
        public required string UserDisplayName { get; init; } = default!;
    }

    // Idea: introduce Special (Rich Text) comments
    // Idea: introduce comment threads (replies to comments)
}