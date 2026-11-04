using CloudCanvas.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CloudCanvas.Application.Posts.Photos.Commands.CreatePhoto
{
    public record CreatePhotoCommand: IRequest<CreatePhotoResult>
    {
        [Required]
        public string? Id { get; init; } = default!;
        [Required]
        public required string UserId { get; init; } = default!;
        [Required]
        public required Creator Creator { get; init; }
        [Required, MaxLength(200)]
        public required string OriginalFilename { get; init; } = default!;
        [Required, Url]
        public required string Location { get; init; } = default!;
        [Required, MaxLength(255)]
        public required string Title { get; init; } = default!;
        
        public string? Caption { get; init; } = default!;
        public string? GalleryId { get; init; } = default!;
        
        public bool CommentsEnabled { get; init; } = true;
        
        [Required, Range(0, long.MaxValue)]
        public long ContentLength { get; init; }
        public List<string>? UserTags { get; init; } = [];
    }
}
