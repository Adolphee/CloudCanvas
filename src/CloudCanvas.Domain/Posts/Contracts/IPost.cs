
using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Reactions;
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

        [Required]
        PostClassification Classification { get; set; }

        List<Reaction> Reactions { get; set; } 
        [NotMapped]
        List<Like> Likes => Reactions.Where(r => r.Type == ReactionType.Like).OfType<Like>().ToList();
        [NotMapped]
        List<Dislike> Dislikes => Reactions.Where(r => r.Type == ReactionType.Dislike).OfType<Dislike>().ToList();
         [NotMapped]
        List<EmojiReaction> EmojiReactions => Reactions.Where(r => r.Type == ReactionType.Emoji).OfType<EmojiReaction>().ToList();
        List<Comment> Comments { get; set; }
        bool CommentsEnabled { get; set; }
        long ContentLength { get; set; }
        List<string> UserTags { get; set; }
    }
}
