
using CloudCanvas.Domain.Abstractions;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Reactions;
using CloudCanvas.Domain.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudCanvas.Domain.Posts.Contracts
{

    public interface IPost : ILikeable, IDisLikeable, IHasTimestamps, IPublishable, IDeletable, IReportable
    {
        [Required]
        public string? Id { get; set; }
        [Required]
        public string? UserId { get; set; }
        public string? Url { get; set; }
        [Required]
        public AppUser? Author { get; set; }

        [Required]
        public PostClassification Classification { get; set; }

        internal List<Reaction> Reactions { get; set; } 
        [NotMapped]
        public List<Like> Likes => Reactions.Where(r => r.Type == ReactionType.Like).OfType<Like>().ToList();
        [NotMapped]
        public List<Dislike> Dislikes => Reactions.Where(r => r.Type == ReactionType.Dislike).OfType<Dislike>().ToList();
        [NotMapped]
        public List<EmojiReaction> EmojiReactions => Reactions.Where(r => r.Type == ReactionType.Emoji).OfType<EmojiReaction>().ToList();
        public List<Comment> Comments { get; set; }
        public bool CommentsEnabled { get; set; }
        public long ContentLength { get; set; }
        //public string? Description { get; set; }
        //public string? DisplayName { get; set; }
        //public string? OriginalFilename { get; set; }
        //public string? Title { get; set; }
        public List<string> UserTags { get; set; }
    }
}
