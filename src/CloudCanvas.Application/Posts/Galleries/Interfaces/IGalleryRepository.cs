using CloudCanvas.Application.Abstractions.Persistence;
using CloudCanvas.Domain.Posts.Entities;

namespace CloudCanvas.Application.Posts.Galleries.Interfaces
{
    public interface IGalleryRepository: IPostRepository<Gallery>
    {
    }
}
