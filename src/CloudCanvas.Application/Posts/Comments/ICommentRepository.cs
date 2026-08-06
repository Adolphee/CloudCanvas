using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Posts.Comments
{
    public interface ICommentRepository
    {   
        Task<IEnumerable<Comment>> GetAllCommentsAsync(string postId, CancellationToken cancellation);
        Task<string?> AddCommentAsync(Comment comment, CancellationToken cancellation = default);
        Task<bool> EditCommentAsync(string id, string text, CancellationToken cancellation = default);
        Task<bool> DeleteCommentAsync(string id, bool softDelete = true, CancellationToken cancellation = default);
        Task<Comment?> SingleAsync(string id, CancellationToken cancellation = default);
    }
}
