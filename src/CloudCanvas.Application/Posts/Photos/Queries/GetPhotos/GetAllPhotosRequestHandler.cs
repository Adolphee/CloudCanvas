using CloudCanvas.Application.Common.Constants;
using IPhotoProjection = CloudCanvas.Application.Posts.Photos.Interfaces.IPhotoProjectionStore;
using MediatR;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotos
{

    public sealed class GetAllPhotosRequestHandler(IPhotoProjection store): IRequestHandler<GetAllPhotosQuery, GetAllPhotosResult>
    {
        private readonly IPhotoProjection _store = store;

        public async Task<GetAllPhotosResult> Handle(GetAllPhotosQuery query, CancellationToken cancellationToken = default)
        {
            var photos = await _store.GetAllAsync(query.ContainerName ?? Projection.Containers.UserPhotos, cancellationToken);
            return new GetAllPhotosResult(photos);
        }
    }
}
