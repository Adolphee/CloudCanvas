using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Application.Posts.Galleries.Interfaces;
using CloudCanvas.Domain.Posts.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudCanvas.Infrastructure.Persistence.Repositories
{
    public class GalleryRepository (CCDBContext context) : IGalleryRepository
    {
        private readonly CCDBContext _context = context;

        public async Task<bool> DeleteAsync(string id, bool softDelete = true, CancellationToken cancellation = default)
        {
            if(await ExistsAsync(id, cancellation))
            {
                var gallery = await GetByIdAsync(id, cancellation);
                if (gallery != null)
                {
                    if (softDelete)
                    {
                        gallery.DeletedOn = DateTime.UtcNow;
                        _context.Galleries.Update(gallery);
                    }
                    else
                    {
                        _context.Galleries.Remove(gallery);
                    }
                    await _context.SaveChangesAsync(cancellation);
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ExistsAsync(string id, CancellationToken cancellation = default)
        {
            return await _context.Galleries.AnyAsync(g => g.Id == id, cancellation);
        }

        public async Task<Gallery?> GetByIdAsync(string id, CancellationToken cancellation = default)
        {
            return await _context.Galleries.FirstOrDefaultAsync(g => g.Id == id, cancellation);
        }

        public async Task<string?> SaveAsync(Gallery gallery, CancellationToken cancellation = default)
        {
            if(await ExistsAsync(gallery.Id, cancellation))
            {
                return null;
            }
            else
            {
                await _context.Galleries.AddAsync(gallery, cancellation);
                await _context.SaveChangesAsync(cancellation);
                return gallery.Id;
            }
        }

        public async Task<bool> UpdateAsync(Gallery gallery, CancellationToken cancellation = default)
        {
            if (await ExistsAsync(gallery.Id, cancellation))
            {
                _context.Galleries.Update(gallery);
                await _context.SaveChangesAsync(cancellation);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
