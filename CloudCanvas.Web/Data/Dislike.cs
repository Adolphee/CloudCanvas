using CloudCanvas.Web.Interfaces;

namespace CloudCanvas.Web.Data
{
    public class Dislike: Reaction
    {
        public Dislike(): base()
        {
            Type = ReactionType.Dislike;
        }
    }
}
