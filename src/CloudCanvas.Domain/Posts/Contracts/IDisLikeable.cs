using CloudCanvas.Domain.Reactions;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface IDisLikeable
    {
        public bool Dislike(string userId);
        public bool RemoveDisLike(Dislike dislike);
        public int DislikesCount();
        public bool IsDislikedBy(string userId);
    }
}
