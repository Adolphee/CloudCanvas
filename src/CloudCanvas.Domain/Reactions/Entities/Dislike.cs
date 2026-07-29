using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Domain.Reactions.Entities
{
    public class Dislike: Reaction
    {
        public Dislike(): base()
        {
            Type = ReactionType.Dislike;
        }
    }
}
