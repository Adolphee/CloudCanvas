namespace CloudCanvas.Web.Data
{
    public enum ReactionType
    {
        Like, Dislike, Emoji
    }

    public class Reaction
    {
        public Guid? Id { get; set; }
        public required string? UserId { get; set; }
        public ApplicationUser? User { get; set; } = default!;
        public required Guid? PostId { get; set; }
        public Post? Post { get; set; } = default!;
        public ReactionType Type { get; set; }
    }

}