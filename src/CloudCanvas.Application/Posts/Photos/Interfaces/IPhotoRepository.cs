using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Domain.Posts.Entities;
using CloudCanvas.Domain.Thumbnail;

namespace CloudCanvas.Application.Posts.Photos.Interfaces
{
    public interface IPhotoRepository: IPostRepository<Photo>
    {
        Task<bool> SaveThumbnailAsync(PhotoThumbnail thumnail, CancellationToken cancellation = default);
    }
}
