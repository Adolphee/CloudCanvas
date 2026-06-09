using CloudCanvas.Web.Data;

namespace CloudCanvas.Web.Interfaces
{
    public interface IDisLikeable
    {
        public Dislike? DisLike(ApplicationUser user);
        public bool RemoveDisLike(Dislike dislike);
        public int DisLikesCount();
        public bool IsDisLikedBy(ApplicationUser user);
    }
}
