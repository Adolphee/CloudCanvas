using CloudCanvas.Application.Posts.Photos;

namespace CloudCanvas.Application.Posts.Queries.GetAllPosts
{
    public record GetAllPostsQueryResult
    {
        public record GetAllPhotosQueryResult(List<PhotoDTO> posts)
        {
            public int Count { get; set; }
            public List<PhotoDTO> Posts = posts;
        }

        public record GetUserPhotosQueryResult : GetAllPhotosQueryResult
        {
            public GetUserPhotosQueryResult(List<PhotoDTO> posts) : base(posts) { }
        }
    }
}
