using CloudCanvas.Application.Posts.Photos;
using System;
using System.Collections.Generic;
using System.Text;

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
