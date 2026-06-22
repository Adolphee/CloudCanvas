using CloudCanvas.Domain.Common;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.Posts.ValueObjects;
using CloudCanvas.Domain.User;


namespace CloudCanvas.Domain.Reactions
{
    public class Reaction: TimeStamped
    {
        public string? Id { get; set; }
        public ReactionType Type { get; set; }
        public string UserId { get; set; } = default!;
        public IAppUser User { get; set; } = default!;
        public string? PostId { get; set; }
        public Post Post { get; set; } = default!;
    }

}