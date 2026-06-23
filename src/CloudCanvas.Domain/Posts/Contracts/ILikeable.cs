using CloudCanvas.Domain.Reactions;
using CloudCanvas.Domain.User;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface ILikeable
    {
        public Like Like(AppUser user);
        public bool UnLike(AppUser user);
        public int LikesCount();
        public bool IsLikedBy(AppUser user);
    }
}