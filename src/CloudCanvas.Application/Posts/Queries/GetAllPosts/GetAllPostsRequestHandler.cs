using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Domain.Posts.ValueObjects;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using IPost = CloudCanvas.Domain.Posts.Contracts.IPost;
using IRepo = CloudCanvas.Application.Abstractions.Persistence.IPostsRepository<CloudCanvas.Domain.Posts.Contracts.IPost>;

namespace CloudCanvas.Application.Posts.Queries.GetAllPosts
{
    public record GetAllPostsQuery
    {
        public required string? UserId { get; set; }
        public required string? ContainerName { get; set; }
        public PostCategory Type { get; set; }
    }

    public record GetAllPostsQueryResult
    {
        public List<IPost> Posts { get; internal set; }
    }

    public class GetAllPostsRequestHandler<T>(IRepo client) where T: IPost
    {
        private readonly IRepo _cosmos = client;

        public async Task<GetAllPostsQueryResult> Handle(GetAllPostsQuery query)
        {
            return new GetAllPostsQueryResult
            {
                Posts = await _cosmos.ListPostsAsync(query?.ContainerName ?? CloudCosmos.Containers.BlobMeta)
            };
        }
    }
}
