using CloudCanvas.Web.Interfaces;

namespace CloudCanvas.Web.Data
{
    public enum ReactionType
    {
        Like, Dislike, Emoji
    }

    public class Reaction: TimeStamped
    {
        public string? Id { get; set; }
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
        public string? PostId { get; set; }
        public Post Post { get; set; } = default!;
        public ReactionType Type { get; set; }
    }

}