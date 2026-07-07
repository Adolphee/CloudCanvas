using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Posts;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotos
{
    public record GetAllPhotosResult(List<PhotoDTO> posts)
    {
        public int Count => Posts.Count;
        public List<PhotoDTO> Posts = posts;
    }
}