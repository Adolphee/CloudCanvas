using CloudCanvas.Application.Abstractions.Projection;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotoByKey
{
    public sealed record GetPhotoByKeyQuery(ProjectionKey LookupKey) : IRequest<GetPhotoByKeyResult>;
}
