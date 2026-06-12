using CloudCanvas.Web.Data;

namespace CloudCanvas.Web.Interfaces
{
    public interface IDisLikeable
    {
        public Dislike? Dislike(ApplicationUser user);
        public bool RemoveDisLike(Dislike dislike);
        public int DisLikesCount();
        public bool IsDislikedBy(ApplicationUser user);
    }
}
