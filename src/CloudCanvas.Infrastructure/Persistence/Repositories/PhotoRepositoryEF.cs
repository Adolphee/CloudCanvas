using CloudCanvas.Application.Posts.Photos.Interfaces;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Thumbnail;
using Microsoft.EntityFrameworkCore;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public class PhotoRepositoryEF(CCDBContext ctx) : IPhotoRepository
    {
        private readonly CCDBContext _contex = ctx;
        public async Task<string> SaveAsync(Photo photo, CancellationToken cancellation = default)
        {
            string id = default!;
            if (await ExistsAsync(photo.Id!, cancellation)) id = _contex.Update(photo).Entity.Id;
            else  id = _contex.Photos.Add(photo).Entity.Id;
            await _contex.SaveChangesAsync(cancellation);
            return id!;
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellation = default) 
            => await _contex.Photos.AnyAsync(x => x.Id == id, cancellation);


        public async Task<Photo?> GetByIdAsync(string id, CancellationToken cancellation = default)
            => await _contex.Photos.Include(p => p.Thumbnails).FirstOrDefaultAsync(p => p.Id == id, cancellation);

        public async Task<bool> UpdateAsync(Photo photo, CancellationToken cancellation = default)
        {
            _contex.Update(photo);
            var res = await _contex.SaveChangesAsync(cancellation);
            return res > 0;
        }

        public async Task<bool> DeleteAsync(string id, bool softDelete = true, CancellationToken cancellation = default)
        {
            var photo = await _contex.Photos.FindAsync(id, cancellation);
            if (photo != null)
            {
                if (softDelete)
                {
                    photo.SetDeletedOn();
                    photo.SetModifiedOn();
                    _contex.Update(photo); 
                } else _contex.Remove(photo);
                return await _contex.SaveChangesAsync(cancellation) > 0;
            } return false;
        }

        public async Task<bool> SaveThumbnailAsync(PhotoThumbnail thumnail, CancellationToken cancellation = default)
        {
            _contex.Thumbnails.Add(thumnail);
            return await _contex.SaveChangesAsync(cancellation) == 1;
        }

        public Task<List<Photo>> GetPhotosByIdsAsync(List<string> photos, CancellationToken cancellation = default)
        =>  _contex.Photos.Where(g => photos.Contains(g.Id!)).ToListAsync(cancellation);
    }
}
