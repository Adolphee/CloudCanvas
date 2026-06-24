using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Domain.Reactions
{
    public record class EmojiReaction: Reaction
    {
        public string? EmojiValue { get; set; }
        EmojiReaction(string value) : base()
        {
            EmojiValue = value;
            Type = ReactionType.Emoji;
        }
    }
}
