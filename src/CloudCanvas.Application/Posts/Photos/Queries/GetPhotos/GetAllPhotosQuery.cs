using CloudCanvas.Domain.Common.Enums;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotos
{
    public record GetAllPhotosQuery : IRequest<GetAllPhotosResult>
    {
        public string? UserId { get; set; }
        public string? ContainerName { get; set; } = CloudCosmos.Containers.UserPhotos;
        public PostClassification Type { get; set; } = PostClassification.Photo;
    }

}
