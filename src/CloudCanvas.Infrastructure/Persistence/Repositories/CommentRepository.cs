using CloudCanvas.Application.Posts.Comments;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public class CommentRepository(CCDBContext context, ILogger<CommentRepository> logger) : ICommentRepository
    {
        private readonly CCDBContext _context = context;
        private readonly ILogger<CommentRepository> logger = logger;
        public async Task<string?> AddCommentAsync(Comment comment, CancellationToken cancellation = default)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(cancellation);
            return comment.Id;
        }

        public async Task<bool> DeleteCommentAsync(string id, bool softDelete = true, CancellationToken cancellation = default)
        {
            _context.Comments.Remove(new Comment { Id = id });
            return await _context.SaveChangesAsync(cancellation) > 0;
        }

        public async Task<bool> EditCommentAsync(string id, string text, CancellationToken cancellation = default)
        {
            Comment? comment = await _context.Comments.FindAsync(id, cancellation);
            if (comment == null) return false;

            comment.Text = text;
            comment.SetModifiedOn();
            return await _context.SaveChangesAsync(cancellation) > 0;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync(string postId, CancellationToken cancellation)
        {
            return await _context.Comments.Where(c => c.PostId == postId).ToListAsync(cancellation);
        }

        public async Task<Comment?> SingleAsync(string id, CancellationToken cancellation = default) => await _context.Comments.FindAsync(id, cancellation);
    }
}
