namespace CloudCanvas.Domain.Reactions
{
    public class EmojiReaction: Reaction
    {
        public string? EmojiValue { get; set; }
        EmojiReaction() : base()
        {
            Type = ReactionType.Emoji;
        }
    }
}
