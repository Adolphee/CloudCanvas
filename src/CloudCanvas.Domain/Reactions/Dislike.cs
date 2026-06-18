namespace CloudCanvas.Domain.Reactions
{
    public class Dislike: Reaction
    {
        public Dislike(): base()
        {
            Type = ReactionType.Dislike;
        }
    }
}
