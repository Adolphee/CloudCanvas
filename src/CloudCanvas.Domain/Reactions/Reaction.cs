using CloudCanvas.Domain.Common;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CloudCanvas.Domain.Reactions
{
    public record class Reaction: TimeStamped
    {
        [Required]
        public string? Id { get; set; }
        public ReactionType Type { get; set; }
        public string UserId { get; set; } = default!;
        [Required, NotMapped]
        public string? PostId { get; set; }
        public Post Post { get; set; } = default!;
    }

}