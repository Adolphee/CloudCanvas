using CloudCanvas.Domain.Common;
using CloudCanvas.Domain.User;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Domain.Posts;


namespace CloudCanvas.Domain.Reactions
{
    public enum ReactionType
    {
        Like, Dislike, Emoji
    }

    public class Reaction: TimeStamped
    {
        public string? Id { get; set; }
        public string UserId { get; set; } = default!;
        public IAppUser User { get; set; } = default!;
        public string? PostId { get; set; }
        public Post Post { get; set; } = default!;
        public ReactionType Type { get; set; }
    }

}