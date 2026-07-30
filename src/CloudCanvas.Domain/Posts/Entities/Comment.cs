using CloudCanvas.Domain.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Domain.Posts.Entities
{
    public class Comment: AuditableEntity
    {
        public required string Id { get; set; }
        [MaxLength(255), Required]
        public string Text { get; set; } = default!;
        [Required]
        public string PostId { get; set; } = default!;
        public Post Post { get; set; } = default!;
        [Required]
        public string UserId { get; set; } = default!;

        [NotMapped]
        public string UserName { get; set; } = default!;
    }

    // Idea: introduce Special (Rich Text) comments
    // Idea: introduce comment threads (replies to comments)
}