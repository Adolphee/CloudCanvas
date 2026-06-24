using CloudCanvas.Domain.Common;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using CloudCanvas.Domain.User;


namespace CloudCanvas.Domain.Reactions
{
    public record class Reaction: TimeStamped
    {
        public string? Id { get; set; }
        public ReactionType Type { get; set; }
        public string UserId { get; set; } = default!;
        public AppUser User { get; set; } = default!;
        public string? PostId { get; set; }
        public Post Post { get; set; } = default!;
    }

}