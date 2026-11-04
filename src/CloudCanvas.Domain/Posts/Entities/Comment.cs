
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudCanvas.Domain.Entities;
namespace CloudCanvas.Domain.Posts.Entities
{
    public class Comment : ReactionTarget
    {
        [MaxLength(255), Required]
        public string Text { get; set; } = default!;
        [Required]
        public string PostId { get; init; } = default!;
        public Post Post { get; set; } = default!;

        [NotMapped]
        public string UserDisplayName { get; set; } = default!;
    }

    // Idea: introduce Special (Rich Text) comments
    // Idea: introduce comment threads (replies to comments)
}