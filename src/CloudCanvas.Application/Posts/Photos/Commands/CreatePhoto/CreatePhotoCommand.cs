using CloudCanvas.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto
{
    public record CreatePhotoCommand: IRequest<CreatePhotoResult>
    {
        public string? Id { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string? Caption { get; set; } = default!;
        public string OriginalFilename { get; set; } = default!;
        [Url]
        public required string Location { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string? GalleryId { get; set; } = default!;
        public bool CommentsEnabled { get; set; } = true;
        public PostClassification Classification { get; set; } = PostClassification.Photo;
        public long ContentLength { get; set; }
        public List<string>? UserTags { get; set; } = new();
        public Creator? Creator = null;
        public string ContainerName = BStorage.Containers.Uploads;
    }
}
