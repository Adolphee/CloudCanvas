using CloudCanvas.Domain.Reactions;
using CloudCanvas.Domain.User;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IDisLikeable
    {
        public Dislike? Dislike(AppUser user);
        public bool RemoveDisLike(Dislike dislike);
        public int DisLikesCount();
        public bool IsDislikedBy(AppUser user);
    }
}
