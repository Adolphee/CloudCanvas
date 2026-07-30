
using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Domain.Posts.Contracts
{
    public interface ICommentable
    {
        public bool AddComment(Comment text);
        public bool RemoveComment(Comment text);
    }
}