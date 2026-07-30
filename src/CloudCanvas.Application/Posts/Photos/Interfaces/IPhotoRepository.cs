using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Thumbnail;

namespace CloudCanvas.Application.Posts.Photos.Interfaces
{
    public interface IPhotoRepository
    {
        Task<string?> SaveAsync(Photo post, CancellationToken cancellation = default);
        Task<bool> UpdateAsync(Photo post, CancellationToken cancellation = default);
        Task<bool> DeleteAsync(string id, bool softDelete = true, CancellationToken cancellation = default);
        Task<Photo?> GetByIdAsync(string id, CancellationToken cancellation = default);
        Task<bool> ExistsAsync(string id, CancellationToken cancellation = default);

        Task<bool> SaveThumbnailAsync(PhotoThumbnail thumnail, CancellationToken cancellation = default);
    }
}
