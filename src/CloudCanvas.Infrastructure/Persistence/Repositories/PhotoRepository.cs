using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Thumbnail;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using CloudCanvas.Application.Common.Exceptions;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public sealed class PhotoRepository(CCDBContext ctx) : IPhotoRepository
    {
        private readonly CCDBContext _contex = ctx;
        public async Task<string> SaveAsync(Photo photo, CancellationToken cancellation = default)
        {
            string id = default!;
            if (await ExistsAsync(photo.Id!, cancellation)) id = _contex.Update(photo).Entity.Id;
            else id = _contex.Photos.Add(photo).Entity.Id;
            await _contex.SaveChangesAsync(cancellation);
            return id!;
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellation = default)
            => await _contex.Photos.AsNoTracking().AnyAsync(x => x.Id == id, cancellation);

        public async Task<Photo?> GetByIdAsync(string id, bool asNoTracking = true, CancellationToken cancellation = default)
        {
            var res = asNoTracking 
                ? _contex.Photos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellation)
                : _contex.Photos
            .Include(p => p.Thumbnails.OrderByDescending(t => t.Size).Take(3))
            .Include(p => p.Comments.Take(10).OrderByDescending(c => c.CreatedOn))
            .Include(p => p.Gallery)
            .Include(p => p.Reactions).OrderByDescending(p => p.CreatedOn)
            .FirstOrDefaultAsync(p => p.Id == id, cancellation);

            return await res;
        }

        public async Task<bool> UpdateAsync(Photo photo, CancellationToken cancellation = default)
        {
            if(await ExistsAsync(photo.Id!, cancellation))
            {
                photo.SetModifiedOn();
                _contex.Photos.Update(photo);
                return await _contex.SaveChangesAsync(cancellation) > 0;
            } else throw new PostNotFoundException($"Photo with id {photo.Id} does not exist when update is requested.");
        }

        public async Task<bool> DeleteAsync(string id, bool softDelete = true, CancellationToken cancellation = default)
        {
            var photo = await _contex.Photos.FindAsync(id, cancellation);
            if (photo != null)
            {
                if (softDelete)
                {
                    photo.SetDeletedOn();
                    _contex.Update(photo);
                }
                else _contex.Remove(photo);
                return await _contex.SaveChangesAsync(cancellation) > 0;
            }
            return false;
        }

        public async Task<bool> SaveThumbnailAsync(PhotoThumbnail thumnail, CancellationToken cancellation = default)
        {
            _contex.Thumbnails.Add(thumnail);
            return await _contex.SaveChangesAsync(cancellation) == 1;
        }

        public async Task<List<Photo>> GetPhotosByIdsAsync(List<string> photos, CancellationToken cancellation = default)
        => await _contex.Photos.AsNoTracking().Where(g => photos.Contains(g.Id!)).ToListAsync(cancellation);
    }
}
