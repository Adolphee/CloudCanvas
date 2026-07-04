using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Application.Posts.DTOs;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using Mapster;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Cosmos.IPostsRepositoryCosmos<CloudCanvas.Domain.Posts.Contracts.IPost>;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetUserPhotosRequestHandler(ICosmosRepo cosmos)
    {
        private ICosmosRepo _cosmos = cosmos;
        public async Task<GetUserPhotosQueryResult> Handle(GetUserPhotosQuery query)
        {
            var posts = await _cosmos.GetUserPhotosAsync(query.UserId, query?.ContainerName ?? CloudCosmos.Containers.BlobMeta);
            return new GetUserPhotosQueryResult(posts);
        }
    }
}
