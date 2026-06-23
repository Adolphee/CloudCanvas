using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Domain.Reactions
{
    public class Like: Reaction
    {
        public Like(): base()
        {
            Type = ReactionType.Like;
        }
    }
}