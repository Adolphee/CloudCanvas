using CloudCanvas.Domain.Enums;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotos
{
    public record GetAllPhotosQuery : IRequest<GetAllPhotosResult>
    {
        public string? UserId { get; set; }
        public string? ContainerName { get; set; } = Projection.Containers.UserPhotos;
        public PostClassification Type { get; set; } = PostClassification.Photo;
    }
}
