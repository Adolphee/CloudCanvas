
namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface ILikeable
    {
        public bool Like(string userId);
        public bool UnLike(string userId);
        public int LikesCount();
        public bool IsLikedBy(string userId);
    }
}