using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Domain.Reactions.Entities
{
    public class Like: Reaction
    {
        public Like(): base()
        {
            Type = ReactionType.Like;
        }
    }
}