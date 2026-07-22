using CloudCanvas.Application.Abstractions.Cosmos;
using CloudCanvas.Application.Posts.DTOs;

namespace CloudCanvas.Application.Posts.Photos.Interfaces
{
    public interface IPhotoProjectionStore: IProjectionStoreBase<PhotoDTO>
    {
    }
}
