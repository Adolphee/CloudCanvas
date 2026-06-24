using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Domain.Reactions
{
    public record class Dislike: Reaction
    {
        public Dislike(): base()
        {
            Type = ReactionType.Dislike;
        }
    }
}
