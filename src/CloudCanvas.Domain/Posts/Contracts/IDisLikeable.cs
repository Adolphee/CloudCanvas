using CloudCanvas.Domain.Reactions;
using CloudCanvas.Domain.User;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IDisLikeable
    {
        public bool Dislike(string userId);
        public bool RemoveDisLike(Dislike dislike);
        public int DisLikesCount();
        public bool IsDislikedBy(string userId);
    }
}
