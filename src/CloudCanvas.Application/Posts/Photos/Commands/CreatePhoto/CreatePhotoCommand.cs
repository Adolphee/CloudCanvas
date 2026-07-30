using CloudCanvas.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto
{
    public record CreatePhotoCommand: IRequest<CreatePhotoResult>
    {
        public string? Id { get; init; } = default!;
        public required string UserId { get; init; } = default!;
        public required Creator Creator { get; init; }
        public required string OriginalFilename { get; init; } = default!;
        [Url]
        public required string Location { get; init; } = default!;
        public required string Title { get; init; } = default!;
        
        public string? Caption { get; init; } = default!;
        public string? GalleryId { get; init; } = default!;
        
        public bool CommentsEnabled { get; init; } = true;
        public PostClassification Classification { get; init; } = PostClassification.Photo;
        public long ContentLength { get; init; }
        public List<string>? UserTags { get; init; } = [];
    }
}
