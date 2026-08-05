using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Thumbnail;

namespace CloudCanvas.Application.Posts.Photos.Interfaces
{
    public interface IPhotoRepository: IPostRepository<Photo>
    {
        Task<List<Photo>> GetPhotosByIdsAsync(List<string> photos, CancellationToken cancellationToken);
        Task<bool> SaveThumbnailAsync(PhotoThumbnail thumnail, CancellationToken cancellation = default);
    }
}
