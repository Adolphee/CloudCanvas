using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CloudCanvas.Domain.Reactions.Entities
{
    public class Reaction: AuditableEntity
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