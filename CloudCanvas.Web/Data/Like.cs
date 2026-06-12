using CloudCanvas.Web.Interfaces;

namespace CloudCanvas.Web.Data
{
    public class Like: Reaction
    {
        public Like(): base()
        {
            Type = ReactionType.Like;
        }
    }
}