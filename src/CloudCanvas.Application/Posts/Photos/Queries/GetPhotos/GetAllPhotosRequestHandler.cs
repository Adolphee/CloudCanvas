using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using ICosmosRepo = CloudCanvas.Application.Abstractions.Cosmos.IPostsRepositoryCosmos<CloudCanvas.Domain.Posts.Contracts.IPost>;
using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Application.Posts.DTOs;
using Mapster;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetAllPhotosQuery: GetAllPostsQuery
    {
    }

    public sealed class GetAllPhotosRequestHandler(ICosmosRepo client)
    {
        private readonly ICosmosRepo _cosmos = client;

        public async Task<List<PhotoDTO>> Handle(GetAllPostsQuery query) => await _cosmos.GetPhotosAsync(query.ContainerName ?? CloudCosmos.Containers.UserPhotos);
    }
}
