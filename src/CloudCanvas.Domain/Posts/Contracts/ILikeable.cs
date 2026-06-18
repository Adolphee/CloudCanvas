using CloudCanvas.Domain.Reactions;
using CloudCanvas.Domain.User;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface ILikeable
    {
        public Like Like(IAppUser user);
        public bool UnLike(IAppUser user);
        public int LikesCount();
        public bool IsLikedBy(IAppUser user);
    }
}