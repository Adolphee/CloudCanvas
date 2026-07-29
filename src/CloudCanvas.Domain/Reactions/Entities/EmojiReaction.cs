using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Domain.Reactions.Entities
{
    public class EmojiReaction: Reaction
    {
        public string? EmojiValue { get; set; }
        EmojiReaction(string value) : base()
        {
            EmojiValue = value;
            Type = ReactionType.Emoji;
        }
    }
}
