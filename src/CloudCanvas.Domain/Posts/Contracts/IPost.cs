
using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Reactions.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Domain.Posts.Contracts
{

    public interface IPost : ILikeable, IDisLikeable, IHasTimestamps, IPublishable, IDeletable, IReportable
    {
        [Required]
        string? Id { get; set; }
        [Required]
        string UserId { get; set; }
        string? Location { get; set; }

        List<Reaction> Reactions { get; set; } 
        [NotMapped]
        List<Like> Likes => [.. Reactions.Where(r => r.Type == ReactionType.Like).OfType<Like>()];
        [NotMapped]
        List<Dislike> Dislikes => [.. Reactions.Where(r => r.Type == ReactionType.Dislike).OfType<Dislike>()];
        [NotMapped]
        List<EmojiReaction> EmojiReactions => [.. Reactions.Where(r => r.Type == ReactionType.Emoji).OfType<EmojiReaction>()];
        List<Comment> Comments { get; set; }
        bool CommentsEnabled { get; set; }
        long ContentLength { get; set; }
        List<string> UserTags { get; set; }
    }
}
