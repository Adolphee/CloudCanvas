using CloudCanvas.Domain.Reactions.Entities;

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
