using CloudCanvas.Application.Posts.Commands.UpdatePost;
using CloudCanvas.Domain.Common.Enums;
using System.ComponentModel.DataAnnotations;
namespace CloudCanvas.Application.Posts.DTOs
{
    public class GalleryItemDTO: UpdateBasicPostInfoRequest
    {
        [Required]
        public string Location { get; set; } = default!;
        [Required]
        public string OriginalFilename { get; set; } = default!;
        public Dictionary<ThumbnailSize, string> Thumbnails { get; set; } = new(); // BlobUrls for the thumbnails
        public string? UploadedBy { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public long ContentLength { get; set; }
    }
}
