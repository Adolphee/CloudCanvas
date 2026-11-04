using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Enums;
using CloudCanvas.Domain.Reactions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IPost : IUserOwned, IReactionTarget, IPublishable, IDeletable, IReportable
    {
        string Location { get; set; }
        bool CommentsEnabled { get; set; }
        long ContentLength { get; set; }
        List<string> UserTags { get; set; }

        List<Reaction> Reactions { get; init; } 
        [NotMapped]
        List<Like> Likes => [.. Reactions.Where(r => r.Type == ReactionType.Like).OfType<Like>()];
        [NotMapped]
        List<Dislike> Dislikes => [.. Reactions.Where(r => r.Type == ReactionType.Dislike).OfType<Dislike>()];
        [NotMapped]
        List<EmojiReaction> EmojiReactions => [.. Reactions.Where(r => r.Type == ReactionType.Emoji).OfType<EmojiReaction>()];
    }
}
