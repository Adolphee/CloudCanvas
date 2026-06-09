using CloudCanvas.Web.Data;

namespace CloudCanvas.Web.Interfaces
{
    public interface ICommentable
    {
        public bool AddComment(Comment text);
        public bool RemoveComment(Comment text);
    }
}