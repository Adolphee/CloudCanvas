using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Common;
using CloudCanvas.Application.Common.Exceptions;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public class PostRepositoryBase<T> (CCDBContext ctx, ILogger<T> logger): IPostRepository<T> where T : Post
    {
        protected readonly CCDBContext _context = ctx;
        private readonly ILogger<T> _logger = logger;
        public virtual async Task<bool> DeleteAsync(string id, bool softDelete = true, CancellationToken cancellation = default)
        {
            if (await ExistsAsync(id, cancellation)) // cheap DB call
            {
                var post = await GetByIdAsync(id, true, cancellation); // expensive DB call
                post!.SetDeletedOn();
                if (softDelete) _context.Set<T>().Update(post);
                else _context.Set<T>().Remove(post);
                await _context.SaveChangesAsync(cancellation);
                return true;
            } else throw new PostNotFoundException($"Post with id {id} does not exist when delete is requested.");
        }

        public virtual async Task<bool> ExistsAsync(string id, CancellationToken cancellation = default) 
        => await _context.Set<T>().AnyAsync(g => g.Id == id, cancellation);

        public virtual async Task<T?> GetByIdAsync(string id, bool tracking = true, CancellationToken cancellation = default)
        => tracking 
            ? await _context.Set<T>().FirstOrDefaultAsync(g => g.Id == id, cancellation)
            : await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(g => g.Id == id, cancellation);

        public virtual async Task<string> SaveAsync(T post, CancellationToken cancellation = default)
        {
            if(await ExistsAsync(post.Id!, cancellation)) _context.Set<T>().Update(post);
            else _context.Set<T>().Add(post);
            await _context.SaveChangesAsync(cancellation); // In case other changes happened on the opbject
            return post.Id;
        }

        public virtual async Task<bool> UpdateAsync(T post, CancellationToken cancellation = default)
        {
            if(await ExistsAsync(post.Id!, cancellation))
            {
                post.SetModifiedOn();
                _context.Set<T>().Update(post);
                return await _context.SaveChangesAsync(cancellation) > 0;
            } else throw new PostNotFoundException($"Post with id {post.Id} does not exist when update is requested.");
        }

        public virtual async Task<List<T>> GetManyByIdsAsync(List<string> postIds, CancellationToken cancellation = default)
        => await _context.Set<T>().AsNoTracking().Where(g => postIds.Contains(g.Id!)).ToListAsync(cancellation);
    }
}