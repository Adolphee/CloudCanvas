using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Photos.Queries.GetPhotosByUser
{
    public record GetUserPhotosQuery: IRequest<GetUserPhotosResult>
    {
        [Required, MaxLength(50)]
        public required string UserId { get; init; }

        [Required, MaxLength(100)]
        public required string ContainerName { get; init; }
    }
}
