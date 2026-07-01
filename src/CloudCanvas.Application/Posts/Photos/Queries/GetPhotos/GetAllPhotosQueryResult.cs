using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Domain.Posts;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotos
{
    public record GetAllPhotosQueryResult(List<PhotoDTO> posts)
    {
        public int Count => Posts.Count;
        public List<PhotoDTO> Posts = posts;
    }

    public record GetUserPhotosQueryResult: GetAllPhotosQueryResult
    {
        public GetUserPhotosQueryResult(List<PhotoDTO> posts) : base(posts) {}
    }
}