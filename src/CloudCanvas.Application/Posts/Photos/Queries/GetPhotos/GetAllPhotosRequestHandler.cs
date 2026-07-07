using CloudCanvas.Application.Common.Constants;
using CloudCanvas.Domain.Common.Enums;
using CloudCanvas.Domain.Posts;
using IPhotoProjection = CloudCanvas.Application.Posts.Photos.Interfaces.IPhotoProjectionStore;
using CloudCanvas.Application.Reactions.Common;
using CloudCanvas.Domain.Posts.Contracts;
using CloudCanvas.Application.Posts.DTOs;
using Mapster;
using CloudCanvas.Application.Posts.Photos.Queries.GetPhotos;
using MediatR;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{

    public sealed class GetAllPhotosRequestHandler(IPhotoProjection store): IRequestHandler<GetAllPhotosQuery, GetAllPhotosResult>
    {
        private readonly IPhotoProjection _store = store;

        public async Task<GetAllPhotosResult> Handle(GetAllPhotosQuery query, CancellationToken cancellationToken)
        {
            var photos = await _store.GetAllAsync(query.ContainerName ?? CloudCosmos.Containers.UserPhotos, cancellationToken);
            return new GetAllPhotosResult(photos);
        }
    }
}
