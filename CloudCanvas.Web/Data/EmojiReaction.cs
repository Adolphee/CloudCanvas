namespace CloudCanvas.Web.Data
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
