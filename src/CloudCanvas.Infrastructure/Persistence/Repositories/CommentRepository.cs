using CloudCanvas.Application.Posts.Comments;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public sealed class CommentRepository(CCDBContext context, ILogger<CommentRepository> logger) : ICommentRepository
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
            _context.Comments.Remove(new(){ Id = id, UserId = "", Text = "", PostId = "", UserDisplayName = "" });
            return await _context.SaveChangesAsync(cancellation) > 0;
        }

        public async Task<bool> EditCommentAsync(Comment cmt, CancellationToken cancellation = default)
        {
            Comment? comment = await _context.Comments.FindAsync(cmt.Id, cancellation);
            if (comment == null) return false;

            comment.Text = cmt.Text;
            comment.SetModifiedOn();
            return await _context.SaveChangesAsync(cancellation) > 0;
        }

        public async Task<IEnumerable<Comment>> GetAllCommentsAsync(string postId, CancellationToken cancellation)
        => await _context.Comments.Where(x => x.PostId == postId).AsNoTracking().ToListAsync(cancellation);
    
        public async Task<Comment?> SingleAsync(string id, CancellationToken cancellation = default) 
        => await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellation);
    }
}