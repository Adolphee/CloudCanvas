using CloudCanvas.Web.Data;

namespace CloudCanvas.Web.Interfaces
{
    public interface ILikeable
    {
        public Like Like(ApplicationUser user);
        public bool UnLike(ApplicationUser user);
        public int LikesCount();
        public bool IsLikedBy(ApplicationUser user);
    }
}