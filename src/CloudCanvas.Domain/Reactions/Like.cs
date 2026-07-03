using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Domain.Reactions
{
    public record class Like: Reaction
    {
        public Like(): base()
        {
            Type = ReactionType.Like;
        }
    }
}