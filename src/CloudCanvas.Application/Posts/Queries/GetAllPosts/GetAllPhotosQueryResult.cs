using CloudCanvas.Domain.Posts;

namespace CloudCanvas.Application.Posts.Queries.GetAllPosts
{
    public record GetAllPhotosQueryResult
    {
        public int Count => Posts.Count;
        public List<PhotoDTO> Posts { get; set; } = new();
    }

}