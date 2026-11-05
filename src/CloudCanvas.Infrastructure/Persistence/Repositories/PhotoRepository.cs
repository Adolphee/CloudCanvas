using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Thumbnail;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using CloudCanvas.Application.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public sealed class PhotoRepository(CCDBContext ctx, ILogger<Photo> logger) : PostRepositoryBase<Photo>(ctx, logger), IPhotoRepository
    {
        private readonly CCDBContext _contex = ctx;
        public async Task<bool> SaveThumbnailAsync(PhotoThumbnail thumnail, CancellationToken cancellation = default)
        {
            _contex.Thumbnails.Add(thumnail);
            return await _contex.SaveChangesAsync(cancellation) == 1;
        }
        public override async Task<Photo?> GetByIdAsync(string id, bool asNoTracking = true, CancellationToken cancellation = default)
        {
            var res = asNoTracking 
                ? _contex.Photos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellation)
                : _contex.Photos
            .Include(p => p.Thumbnails.OrderByDescending(t => t.Size).Take(3))
            .Include(p => p.Comments.OrderByDescending(c => c.CreatedOn)).Take(10)
            .Include(p => p.Gallery)
            .Include(p => p.Reactions).OrderByDescending(p => p.CreatedOn)
            .FirstOrDefaultAsync(p => p.Id == id, cancellation);

            return await res;
        }

    }
}
