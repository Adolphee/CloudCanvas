using CloudCanvas.Application.Posts.Commands.UpdatePost;
using CloudCanvas.Domain.Enums;
using System.ComponentModel.DataAnnotations;
namespace CloudCanvas.Application.Posts.DTOs
{
    public record GalleryItemDTO: UpdateBasicPostInfoRequest
    {
        public required string Location { get; init; }
        public required string OriginalFilename { get; init; }
        public Dictionary<ThumbnailSize, string> Thumbnails { get; init; } = new();
        public string? UploadedBy { get; init; }
        public DateTimeOffset LastModified { get; init; } = DateTimeOffset.Now;
        public DateTimeOffset CreatedOn { get; init; } = DateTimeOffset.Now;
        public long ContentLength { get; init; }
    }
}
